using System;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Ratings
{
    public class AdRating : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long AdId { get; set; }
        public Ad Ad { get; set; }
        public string RaterUserId { get; set; }
        public AppUser RaterUser { get; set; }
        public long ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public Rating ComplianceRating { get; set; }
        public Rating QualityRating { get; set; }
        public Rating OverallRating { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime LastUpdatedAtUtc { get; set; }
    }
}
