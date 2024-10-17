using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices.JavaScript;

namespace ZoneProductionDashBoard
{
    public static class DashboardConfig
    {
        public static IConfiguration? Configuration;

        public static string TrelloApiKey 
            => Configuration?.GetValue<string>("TRELLOAPIKEY")?? throw new ArgumentNullException("TrelloApiKey", "TrelloApiKey must be added to config file.");
        
        public static string TrelloUserToken 
            => Configuration?["TRELLOUSERTOKEN"]?? throw new ArgumentNullException("TrelloUserToken", "TrelloUserToken must be added to config file.");
        
        public static string WebhookCallbackUrl
            => Configuration?["WEBHOOKCALLBACKURL"]?? throw new ArgumentNullException("WebhookCallbackURL", "WebhookCallbackURL must be added to config file.");
        
        public static bool EnableWebhooks
            => bool.TryParse(Configuration?["enableWebhooks"], out bool res) ? res : false;

        public static bool UseDevProductionService
            => bool.TryParse(Configuration?["UseDevProductionService"], out bool res) ? res : false;
         
        
        public static void AddConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}