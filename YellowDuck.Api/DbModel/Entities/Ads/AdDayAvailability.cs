using System;

namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class AdDayAvailability : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long AdId { get; set; }
        public Ad Ad { get; set; }
        public DayOfWeek Weekday { get; set; }
    }
}
