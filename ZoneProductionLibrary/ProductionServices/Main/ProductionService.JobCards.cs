namespace ZoneProductionLibrary.ProductionServices.Main;

public partial class ProductionService
{
    public async Task<IEnumerable<JobCard>> GetJobCardsAsync(IProgress<double> progress, IEnumerable<string> boardIds)
    {
        double       report     = 0;
        List<string> enumerable = boardIds.ToList();
        foreach (string boardId in enumerable)
        {
            report += 100d / enumerable.Count();
            progress.Report(report);

            if (!_vanBoards.ContainsKey(boardId)) await GetBoardAsyncById(boardId);
        }

        return GetJobCards(enumerable);
    }
        
    public IEnumerable<JobCard> GetJobCards(IEnumerable<string> boardIds)
    {
        List<JobCard> cards = [];

        foreach (JobCardObject card in _jobCards.Values.Where(x => boardIds.Contains(x.BoardId)))
        {
            JobCard? jobCard = GetJobCard(card);
            
            if (jobCard is not null)
                cards.Add(jobCard);
        }

        return cards.AsEnumerable();
    }

    public IEnumerable<JobCard> GetJobCards(IEnumerable<VanModel> vanTypes)
    {
        List<JobCard> cards = [];

        foreach (JobCardObject card in _jobCards.Values.Where(x => vanTypes.Contains(x.Name.ToVanType())))
        {
            JobCard? jobCard = GetJobCard(card);
            
            if (jobCard is not null)
                cards.Add(jobCard);
        }

        return cards.AsEnumerable();
    }
}