using System;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Ratings
{
    public class UserRating : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RaterUserId { get; set; }
        public AppUser RaterUser { get; set; }
        public long ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public Rating RespectRating { get; set; }
        public Rating CommunicationRating { get; set; }
        public Rating OverallRating { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime LastUpdatedAtUtc { get; set; }
        public string Comment { get; set; }
    }
}
