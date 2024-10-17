using DBLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TrelloDotNet;
using TrelloDotNet.Model.Webhook;
using ZoneProductionLibrary.Models.UpdateData;
using ZoneProductionLibrary.ProductionServices.Base;
using ZoneProductionLibrary.ProductionServices.Main;

namespace ZoneProductionDashBoard
{
    [ApiController, Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private ITrelloActionData _trelloActionDataDB;
        private IProductionService _productionService;
        public WebhookController(ITrelloActionData trelloActionData, IProductionService productionService)
        {
            _trelloActionDataDB = trelloActionData;
            _productionService = productionService;
        }

        private List<string> _handledWebhooks = [];
        private static bool _hasCheckedRequiredWebhooks;

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            if(!DashboardConfig.EnableWebhooks)
                return new OkObjectResult("OK");

            if (!_hasCheckedRequiredWebhooks && _productionService is ProductionService prod)
            {
                _hasCheckedRequiredWebhooks = true;
                await prod.CheckRequiredWebhooksActive();
            }

            TrelloClient client = new TrelloClient(DashboardConfig.TrelloApiKey, DashboardConfig.TrelloUserToken);
            WebhookDataReceiver receiver = new WebhookDataReceiver(client);

            // Check Events
            receiver.BasicEvents.OnCreateCheckItem += CreateCheck;
            receiver.BasicEvents.OnDeleteCheckItem += DeleteCheck;
            receiver.BasicEvents.OnUpdateCheckItemStateOnCard += UpdateCheckItem;
            receiver.BasicEvents.OnUpdateCheckItem += UpdateCheckItem;

            // Checklist Events
            receiver.BasicEvents.OnCopyChecklist += CopyCheckList;
            receiver.BasicEvents.OnRemoveChecklistFromCard += DeleteCheckList;
            receiver.BasicEvents.OnUpdateChecklist += UpdateCheckList;
            receiver.BasicEvents.OnAddChecklistToCard += CreateCheckList;

            // Card Events
            receiver.BasicEvents.OnCreateCard += NewCardCreated;
            receiver.BasicEvents.OnDeleteCard += CardDeleted;
            receiver.BasicEvents.OnCopyCard += CardCopied;
            receiver.BasicEvents.OnUpdateCard += CardUpdated;
            receiver.BasicEvents.OnUpdateCustomFieldItem += CustomFieldUpdated;
            receiver.BasicEvents.OnAddAttachmentToCard += AttachmentAdded;
            //receiver.BasicEvents.OnDeleteAttachmentFromCard
            receiver.BasicEvents.OnAddMemberToCard += MemberAddedToCard;
            receiver.BasicEvents.OnRemoveMemberFromCard += MemberRemovedFromCard;

            // Comment Events
            receiver.BasicEvents.OnCommentCard += CardCommentUpdated;
            receiver.BasicEvents.OnDeleteComment += CardCommentUpdated;
            receiver.BasicEvents.OnUpdateComment += CardCommentUpdated;
            
            //User events
            //receiver.BasicEvents.OnUpdateMember
            //receiver.BasicEvents.OnRemoveMemberFromOrganization
            //receiver.BasicEvents.OnAddMemberToOrganization
            
            // List Events
            //receiver.BasicEvents.OnUpdateList
            //receiver.BasicEvents.OnMoveListToBoard
            //receiver.BasicEvents.OnMoveListFromBoard
            
            //handle board closing
            
            try
            {
                using StreamReader streamReader = new StreamReader(Request.Body);
                string json = await streamReader.ReadToEndAsync();

                if (json != string.Empty)
                {
                    var notification = receiver.ConvertJsonToWebhookNotification(json);

                    Log.Logger.Verbose("Webhook received [{type}]", notification.Action.Type);

                    if (!_handledWebhooks.Contains(notification.Action.Id))
                    {
                        receiver.ProcessJsonIntoEvents(json);
                        _handledWebhooks.Add(notification.Action.Id);
                    }
                    else
                    {
                        Log.Logger.Warning("Webhook action already handled.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception occured during webhook.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult("OK");
        }

        [HttpHead]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> Head()
        {
            if(!DashboardConfig.EnableWebhooks)
                return new OkObjectResult("OK");
            
            if (!_hasCheckedRequiredWebhooks && _productionService is ProductionService prod)
            {
                _hasCheckedRequiredWebhooks = true;
                await prod.CheckRequiredWebhooksActive();
            }
            
            return new OkObjectResult("OK");
        }


        // Check Events
        public static event EventHandler<CheckUpdatedData>? CheckUpdatedEvent;
        public static event EventHandler<CheckCreatedData>? CreateCheckEvent;
        public static event EventHandler<CheckDeletedData>? CheckDeletedEvent;

        private void CreateCheck(WebhookAction args)
        {
            Log.Logger.Debug("{memberName} created new check {checkName}:{cardName}:{boardName}", args.MemberCreator.FullName, args.Data.CheckItem.Name, args.Data.Card.Name, args.Data.Board.Name);
            CreateCheckEvent?.Invoke(null, new CheckCreatedData(args));
        }

        private void UpdateCheckItem(WebhookAction args)
        {
            Log.Logger.Debug("{memberName} Has updated {checkName}:{boardName} as {state}", args.MemberCreator.FullName, args.Data.CheckItem.Name, args.Data.Board.Name, args.Data.CheckItem.State);
            CheckUpdatedEvent?.Invoke(null, new CheckUpdatedData(args));
        }

        private void DeleteCheck(WebhookAction args)
        {
            Log.Logger.Debug("{memberName} deleted check {checkName}:{cardName}:{boardName}", args.MemberCreator.FullName, args.Data.CheckItem.Name, args.Data.Card.Name, args.Data.Board.Name);
            CheckDeletedEvent?.Invoke(null, new CheckDeletedData(args));
        }

        // Checklist events
        public static event EventHandler<CheckListDeletedData>? CheckListDeletedEvent;
        public static event EventHandler<CheckListCreatedData>? CheckListCreatedEvent;
        public static event EventHandler<CheckListCreatedData>? CheckListCopiedEvent;
        public static event EventHandler<CheckListUpdatedData>? CheckListUpdatedEvent;
        private void DeleteCheckList(WebhookAction args)
        {
            Log.Logger.Debug("{memberName} deleted checklist {checkListName} from {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Checklist.Name, args.Data.Card, args.Data.Board.Name);
            CheckListDeletedEvent?.Invoke(null, new CheckListDeletedData(args));
        }

        private void UpdateCheckList(WebhookAction args)
        {
            Log.Logger.Debug("{memberName} Updated checklist {checkListName} on {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Checklist.Name, args.Data.Card.Name, args.Data.Board.Name);
            CheckListUpdatedEvent?.Invoke(null, new CheckListUpdatedData(args));
        }

        private void CreateCheckList(WebhookAction args)
        {
            Log.Logger.Debug("{memberName} Created checklist {checkListName} on {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Checklist.Name, args.Data.Card.Name, args.Data.Board.Name);
            CheckListCreatedEvent?.Invoke(null, new CheckListCreatedData(args));
        }

        private void CopyCheckList(WebhookAction args)
        {
            Log.Logger.Debug("{memberName} copied checklist {checkListName} from {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Checklist.Name, args.Data.Card.Name, args.Data.Board.Name);
            CheckListCopiedEvent?.Invoke(null, new CheckListCreatedData(args));
        }

        // Card Events
        public static event EventHandler<CardUpdatedData>? CardCreatedEvent;
        public static event EventHandler<CardUpdatedData>? CardDeletedEvent;
        public static event EventHandler<CardUpdatedData>? CardUpdatedEvent;
        public static event EventHandler<MemberAddedToCardData>? MemberAddedToCardEvent;
        public static event EventHandler<MemberAddedToCardData>? MemberRemovedFromCardEvent;
        public static event EventHandler<CardUpdatedData>? CCDashboardCardUpdatedEvent;
        public static event EventHandler<CardUpdatedData>? LineMoveCardUpdatedEvent;
        public static event EventHandler<CardUpdatedData>? CardCommentsUpdatedEvent;
        public static event EventHandler<CardUpdatedData>? CustomFieldUpdatedEvent;
        public static event EventHandler<AttachmentAddedData>? AttachmentAddedEvent;
        private void NewCardCreated(WebhookAction args)
        {
            CardCreatedEvent?.Invoke(null, new CardUpdatedData(args));
            Log.Logger.Debug("{memberName} Created new card {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Card.Name, args.Data.Board.Name);
        }

        private void CardDeleted(WebhookAction args)
        {
            CardDeletedEvent?.Invoke(null, new CardUpdatedData(args));
            Log.Logger.Debug("{memberName} Deleted card {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Card.Name, args.Data.Board.Name);
        }
        
        private void CardCopied(WebhookAction args)
        {
            CardCreatedEvent?.Invoke(null, new CardUpdatedData(args));
            Log.Logger.Debug("{memberName} Copied new card {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Card.Name, args.Data.Board.Name);
        }

        private void CardUpdated(WebhookAction args)
        {
            if (args.Data.Board.Id == ProductionService.CCDashboardId)
            {
                CCDashboardCardUpdatedEvent?.Invoke(null, new CardUpdatedData(args));
                Log.Logger.Information("CC Dashboard Card Updated {cardName}", args.Data.Card.Name);
            }
            else if (args.Data.Board.Id == ProductionService.LineMoveBoardId)
            {
                LineMoveCardUpdatedEvent?.Invoke(null, new CardUpdatedData(args));
                Log.Logger.Information("Line move Card Updated {cardName}", args.Data.Card.Name);
            }
            else
            {
                CardUpdatedEvent?.Invoke(null, new CardUpdatedData(args));
                Log.Logger.Debug("{memberName} Updated new card {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Card.Name, args.Data.Board.Name);
            }
        }

        private void MemberAddedToCard(WebhookAction args)
        {
            MemberAddedToCardEvent?.Invoke(null, new MemberAddedToCardData(args));
            Log.Logger.Debug("{memberName} added to {cardName}:{boardName}", args.Data.Member.Name, args.Data.Card.Name, args.Data.Board.Name);
        }
            
        private void MemberRemovedFromCard(WebhookAction args)
        {
            MemberRemovedFromCardEvent?.Invoke(null, new MemberAddedToCardData(args));
            Log.Logger.Debug("{memberName} removed from {cardName}:{boardName}", args.Data.Member.Name, args.Data.Card.Name, args.Data.Board.Name);
        }

        private void CustomFieldUpdated(WebhookAction args)
        {
            CustomFieldUpdatedEvent?.Invoke(null, new CardUpdatedData(args));
            Log.Logger.Debug("{memberName} updated custom field on {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Card.Name, args.Data.Board.Name);
        }
        private void CardCommentUpdated(WebhookAction args)
        {
            CardCommentsUpdatedEvent?.Invoke(null, new CardUpdatedData(args));
            Log.Logger.Debug("{memberName} modified comments in {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Card.Name, args.Data.Board.Name);
        }

        private void AttachmentAdded(WebhookAction args)
        {
            AttachmentAddedEvent?.Invoke(null, new AttachmentAddedData(args));
            Log.Logger.Debug("{memberName} added attachment {attachmentName} to {cardName}:{boardName}", args.MemberCreator.FullName, args.Data.Attachment.Name, args.Data.Card.Name, args.Data.Board.Name);
        }
    }
}
