using TrelloDotNet.Model.Actions;
using TrelloDotNet.Model.Webhook;

namespace DBLibrary.Models
{
    public class CachedTrelloAction
    {
        public string ActionId { get; set; }
        public string BoardId { get; set; }
        public string CardId { get; set; }
        public DateTimeOffset DateOffset { get; set; }
        public string ActionType { get; set; }
        public string MemberId { get; set; }
        public string? Content { get; set; }
        public string? CheckId { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        
        // Dapper needs a blank constructor to parse data
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CachedTrelloAction()
        {
            
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public CachedTrelloAction(TrelloAction action)
        {
            this.ActionId = action.Id;
            this.BoardId = action.Data.Board.Id;
            this.CardId = action.Data.Card.Id;
            this.DateOffset = action.Date;
            this.ActionType = action.Type;
            this.MemberId = action.MemberCreatorId;
            this.Content = action.Data.Text;
            this.CheckId = action.Data.CheckItem?.Id;
            this.DueDate = action.Data.Card.Due;
        }
    }
}
