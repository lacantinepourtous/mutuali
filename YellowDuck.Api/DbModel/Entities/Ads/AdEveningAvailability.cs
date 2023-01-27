using System;

namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class AdEveningAvailability : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long AdId { get; set; }
        public Ad Ad { get; set; }
        public DayOfWeek Weekday { get; set; }
    }
}
