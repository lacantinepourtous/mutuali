using System;

namespace YellowDuck.Api.DbModel.Entities.Notifications
{
    public class ConversationNotification
    {
        public long Id { get; set; }

        public long ConversationId { get; set; }

        public string Body { get; set; }
        public DateTime DateCreated { get; set; }

        public string UserId { get; set; }
        public string NotificationCreator { get; set; }
    }
}
