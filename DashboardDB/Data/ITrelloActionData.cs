using DBLibrary.Models;
using TrelloDotNet.Model.Actions;

namespace DBLibrary.Data
{
    public interface ITrelloActionData
    {
        public static List<string> ActionFilters = ["commentCard", "updateCustomFieldItem", "createCard", "updateCheckItemStateOnCard"];

        public Task InsertTrelloAction(TrelloAction action);
        public Task<IEnumerable<CachedTrelloAction>> InsertTrelloActions(IEnumerable<TrelloAction> actions);
        public Task<IEnumerable<CachedTrelloAction>> GetActions(string boardId);
        public Task DeleteActions(string boardId);
    }
}