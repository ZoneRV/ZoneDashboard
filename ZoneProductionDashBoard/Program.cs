using DBLibrary.Data;
using DBLibrary.DbAccess;
using Radzen;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Globalization;
using ZoneProductionDashBoard;
using ZoneProductionLibrary.Jobs;
using ZoneProductionLibrary.ProductionServices;
using ZoneProductionLibrary.ProductionServices.Base;
using ProductionService = ZoneProductionLibrary.ProductionServices.Main.ProductionService;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-AU");
            //CultureInfo.DefaultThreadCurrentUICulture= new CultureInfo("en-AU");
            
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            
            DashboardConfig.AddConfiguration(builder.Configuration);
            
            Log.Logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(builder.Configuration)
                         .WriteTo.Console(
                             theme: AnsiConsoleTheme.Code,
                             outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
                         .WriteTo.File(
                             "Logs/log-.txt",
                             shared: true,
                             flushToDiskInterval: TimeSpan.FromSeconds(10),
                             rollingInterval: RollingInterval.Day,
                             retainedFileCountLimit: 7,
                             outputTemplate: "[{Level:u3} {Timestamp:HH:mm:ss}] {Message:lj} {NewLine}{Exception}")
                         .CreateLogger();
            
            Log.Logger.Information("Application is starting...");

            if (!Directory.Exists("Logs/CompareReports/"))
                Directory.CreateDirectory("Logs/CompareReports/");

            // Add services to the container.
            builder.Services.AddSerilog();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddBlazorBootstrap();
            builder.Services.AddScoped<StatsService>();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<ContextMenuService>();
            builder.Services.AddScoped<TooltipService>();

            builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
            builder.Services.AddSingleton<ITrelloActionData, TrelloActionData>();
            builder.Services.AddSingleton<IVanIdData, VanIdData>();

            if (DashboardConfig.UseDevProductionService)
                builder.Services.AddSingleton<IProductionService, DevProductionService>();

            else
                builder.Services.AddSingleton<IProductionService, ProductionService>();
                   
            builder.Services.AddControllers();
            
            WebApplication app = builder.Build();

            IProductionService? productionService = app.Services.GetService<IProductionService>();
            
            ArgumentNullException.ThrowIfNull(productionService);

            Task.Run(async () =>
            {
                await productionService.InitializeProductionService();

                if (productionService is ProductionService production)
                {
                    Scheduler.AddTask(new DailyTask(production.ProductionServiceCleanUp, TimeSpan.Zero));
                    
                    if(DashboardConfig.EnableWebhooks)
                    {
                        WebhookController.LineMoveCardUpdatedEvent += production.UpdatedLineMoveBoardInfo;
                        WebhookController.CCDashboardCardUpdatedEvent += production.UpdateCCDashboardInfo;

                        WebhookController.CheckUpdatedEvent += production.UpdateCheck;
                        WebhookController.CreateCheckEvent += production.CreateCheck;
                        WebhookController.CheckDeletedEvent += production.DeleteCheck;

                        WebhookController.CheckListDeletedEvent += production.DeleteCheckList;
                        WebhookController.CheckListUpdatedEvent += production.UpdateCheckList;
                        WebhookController.CheckListCreatedEvent += production.CreateCheckList;
                        WebhookController.CheckListCopiedEvent += production.CopyCheckList;

                        WebhookController.CardCreatedEvent += production.CreateCard;
                        WebhookController.CardDeletedEvent += production.DeleteCard;
                        WebhookController.CardUpdatedEvent += production.UpdateCard;
                        WebhookController.CardCommentsUpdatedEvent += production.UpdateCommentsOnCard;
                        WebhookController.AttachmentAddedEvent += production.AttachmentAdded;
                        WebhookController.MemberAddedToCardEvent += production.MemberAddedToCard;
                        WebhookController.MemberRemovedFromCardEvent += production.MemberRemovedFromCard;

                        WebhookController.CustomFieldUpdatedEvent += production.UpdateCustomFieldItems;
                    }
                }

            }).GetAwaiter().GetResult();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.MapControllers();
            app.UseAntiforgery();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "The application failed to start correctly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}