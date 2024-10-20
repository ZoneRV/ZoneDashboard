using System.Text.RegularExpressions;
using ZoneProductionLibrary.Models;
using ZoneProductionLibrary.Models.Boards;
using ZoneProductionLibrary.Models.Trello;
using ZoneProductionLibrary.Models.ProductionPosition;

namespace ZoneProductionLibrary.Extensions
{
    public static class FilterExtensions
    {
        private static string CleanName(string name) => Regex.Replace(name.ToLower(), "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);

        public static IEnumerable<T> FilterCards<T>(this IEnumerable<T> cards, CardFilterOptions filterOptions) where T : IFilterableCard
        {
            cards = cards.FilterCards(filterOptions.CardAreaOfOrigins);

            if (filterOptions.SearchTerms != null)
            {
                cards = cards.FilterCards(filterOptions.SearchTerms, filterOptions.SearchExactName);
            }

            cards = cards.FilterCards(filterOptions.CardStatuses);

            return cards;
        }


        public static IEnumerable<T> FilterCards<T>(this IEnumerable<T> cards, IEnumerable<string> searchTerms, bool SearchExactName) where T : IFilterableCard
        {
            if (SearchExactName)
                cards.Where(x => searchTerms.Contains(x.Name));

            else
                cards = cards.Where(c => searchTerms.Any(s => CleanName(c.Name).Contains(s.ToLower())));

            return cards;
        }

        public static IEnumerable<T> FilterCards<T>(this IEnumerable<T> cards, IEnumerable<CardStatus> cardStatuses) where T : IFilterableCard
            => cards.Where(x => cardStatuses.Contains(x.CardStatus));
        
        public static IEnumerable<T> FilterCards<T>(this IEnumerable<T> cards, IEnumerable<CardAreaOfOrigin> areaOfOrigins) where T : IFilterableCard
            => cards.Where(x => areaOfOrigins.Contains(x.AreaOfOrigin));

        public static IEnumerable<T> FilterVans<T>(this IEnumerable<T> boards, BoardFilterOptions filterOptions) where T : IFilterableBoard
        {
            boards = boards.FilterVans(filterOptions.vanTypes);

            if (filterOptions.BoardIds != null)
                boards = boards.FilterVans(filterOptions.BoardIds);

            if (filterOptions.ProductionPositions != null)
                boards = boards.FilterVans(filterOptions.ProductionPositions);

            return boards;
        }

        public static IEnumerable<T> FilterVans<T>(this IEnumerable<T> boards, IEnumerable<string> boardIds) where T : IFilterableBoard
            => boards.Where(x => boardIds.Contains(x.Id));

        public static IEnumerable<T> FilterVans<T>(this IEnumerable<T> boards, IEnumerable<VanModel> vanTypes) where T : IFilterableBoard
            => boards.Where(x => vanTypes.Contains(x.VanModel));

        public static IEnumerable<T> FilterVans<T>(this IEnumerable<T> boards, IEnumerable<IProductionPosition> productionPositions) where T : IFilterableBoard
            => boards.Where(x => productionPositions.Contains(x.Position));
    }

    public class CardFilterOptions
    {
        public IEnumerable<CardAreaOfOrigin> CardAreaOfOrigins { get; set; }
        public IEnumerable<RedFlagIssue> RedFlagIssues { get; set; }
        public IEnumerable<string>? SearchTerms { get; set; }
        public bool SearchExactName { get; set; }
        public IEnumerable<CardStatus> CardStatuses { get; set; }

        public CardFilterOptions()
        {
            CardAreaOfOrigins = Enum.GetValues<CardAreaOfOrigin>();
            RedFlagIssues = Enum.GetValues<RedFlagIssue>();
            SearchTerms = null;
            SearchExactName = false;
            CardStatuses = Enum.GetValues<CardStatus>();
        }
    }

    public class BoardFilterOptions
    {
        public IEnumerable<VanModel> vanTypes { get; set; }
        public IEnumerable<string>? BoardIds { get; set; }
        public IEnumerable<IProductionPosition>? ProductionPositions { get; set; }

        public BoardFilterOptions()
        {
            vanTypes = Enum.GetValues<VanModel>().Where(x => x.IsGen2());
            BoardIds = null;
            ProductionPositions = null;
        }
    }
}
