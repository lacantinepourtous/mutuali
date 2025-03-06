using System;

namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class AdAvailabilityRestriction : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long AdId { get; set; }
        public Ad Ad { get; set; }
        public bool Day { get; set; }
        public bool Evening { get; set; }
        public DateTime StartDate { get; set; }
    }
}
