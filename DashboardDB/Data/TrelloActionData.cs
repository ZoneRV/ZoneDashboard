using Dapper;
using DBLibrary.DbAccess;
using DBLibrary.Models;
using System.Data;
using TrelloDotNet.Model.Actions;

namespace DBLibrary.Data
{
    public class TrelloActionData : ITrelloActionData
    {
        private readonly ISqlDataAccess _db;

        public TrelloActionData(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task InsertTrelloAction(TrelloAction action)
            => await _db.SaveData("dbo.spTrelloAction_Insert", new CachedTrelloAction(action));

        public async Task<IEnumerable<CachedTrelloAction>> InsertTrelloActions(IEnumerable<TrelloAction> actions)
        {
            var output = new DataTable();
            List<CachedTrelloAction> results = [];

            output.Columns.Add("ActionId",   typeof(string));
            output.Columns.Add("BoardId",    typeof(string));
            output.Columns.Add("CardId",     typeof(string));
            output.Columns.Add("DateOffset", typeof(DateTimeOffset));
            output.Columns.Add("ActionType", typeof(string));
            output.Columns.Add("MemberId",   typeof(string));
            output.Columns.Add("Content",    typeof(string));
            output.Columns.Add("CheckId",    typeof(string));
            output.Columns.Add("DueDate",    typeof(DateTimeOffset));

            foreach (var action in actions)
            {
                results.Add(new CachedTrelloAction(action));
                
                output.Rows.Add(
                    action.Id, 
                    action.Data.Board.Id, 
                    action.Data.Card.Id, 
                    action.Date,
                    action.Type, 
                    action.MemberCreatorId, 
                    action.Data.Text,
                    action.Data.CheckItem?.Id,
                    action.Data.Card.Due);
            }

            var a = new
            {
                actions = output.AsTableValuedParameter("TrelloActionUDT")
            };

            await _db.SaveData("dbo.spTrelloAction_InsertSet", a);
            
            return results;
        }

        public async Task<IEnumerable<CachedTrelloAction>> GetActions(string boardId)
            => await _db.LoadData<CachedTrelloAction, dynamic>("dbo.spTrelloAction_GetAllOnBoard", new { BoardId = boardId });

        public async Task DeleteActions(string boardId)
            => await _db.SaveData("dbo.spTrelloAction_DeleteOnBoard", new { BoardId = boardId });
    }
}
