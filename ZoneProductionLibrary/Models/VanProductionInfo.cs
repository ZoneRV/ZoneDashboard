using TrelloDotNet.Model.Actions;

namespace ZoneProductionLibrary.Models
{
    public class VanProductionInfo : IFilterableBoard, IEqualityComparer<VanProductionInfo>, IEquatable<VanProductionInfo>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public VanModel VanModel => Name.ToVanType();
        public IProductionPosition Position => PositionHistory.Count > 0 ? PositionHistory.Last().position : new PreProduction();
        public List<(DateTimeOffset date, IProductionPosition position)> PositionHistory { get; set; }
        
        public List<(DateTimeOffset changeDate, DateTimeOffset HandoverDate)> HandoverHistory { get; set; } = [];

        public DateTimeOffset? Handover => HandoverHistory.Any() ? HandoverHistory.Last().HandoverDate : null;
        public HandoverState HandoverState { get; set; } = HandoverState.Unknown;

        public TimeSpan? TimeToHandover => Handover.HasValue ? Handover.Value - DateTimeOffset.Now : null;
        public bool IsInCarPark => Position.IsInCarPark(HandoverState);

        public override string ToString() => $"{Name}:{Id} {Position} {(Handover.HasValue ? Handover.Value.LocalDateTime.ToString("dd/MM/yy") : "")}";

        public VanProductionInfo(string id, string name, List<(DateTimeOffset, IProductionPosition)> positionHistory)
        {
            Id = id;
            Name = name;
            PositionHistory = positionHistory.ToList();
        }

        public VanProductionInfo(string id, string name, List<(DateTimeOffset, IProductionPosition)> positionHistory, List<(DateTimeOffset, DateTimeOffset)> handoverHistory, HandoverState handoverState)
        {
            Id = id;
            Name = name;
            PositionHistory = positionHistory.ToList();
            HandoverHistory = handoverHistory.ToList();
            HandoverState = handoverState;
        }
        
        
        /// <param name="timeSpan">How new the change has to be</param>
        /// <param name="date">How close the handover date has to be</param>
        public bool HasHandoverDateChanged(TimeSpan timeSpan, TimeSpan date, bool ignoreDueVans = true)
        {
            if (HandoverHistory.Count < 2)
                return false;

            DateTimeOffset lastDate = HandoverHistory.TakeLast(2).First().HandoverDate;
            DateTimeOffset newDate = HandoverHistory.Last().HandoverDate;

            if (ignoreDueVans && HandoverState == HandoverState.HandedOver && newDate < DateTimeOffset.Now)
                return false;

            if (lastDate.LocalDateTime.Date == newDate.LocalDateTime.Date)
                return false;

            if (newDate - date > DateTimeOffset.Now || lastDate - date > DateTimeOffset.Now)
                return false;

            return (HandoverHistory.Last().changeDate + timeSpan > DateTimeOffset.Now );
        }
        
        public bool Equals(VanProductionInfo? other) => throw new NotImplementedException();

        public bool Equals(VanProductionInfo? x, VanProductionInfo? y) => throw new NotImplementedException();

        public int GetHashCode(VanProductionInfo obj) => throw new NotImplementedException();
    }

    public enum HandoverState
    {
        Unknown,
        UnhandedOver,
        HandedOver
    }
}
