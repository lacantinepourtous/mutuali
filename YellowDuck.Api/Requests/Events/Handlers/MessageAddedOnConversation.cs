using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Notifications;

namespace YellowDuck.Api.Requests.Events.Handlers
{
    public class MessageAddedOnConversation : INotificationHandler<MessageAdded>
    {
        private readonly AppDbContext db;
        private readonly ILogger<MessageAddedOnConversation> logger;

        public MessageAddedOnConversation(AppDbContext db, ILogger<MessageAddedOnConversation> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public async Task Handle(MessageAdded notification, CancellationToken cancellationToken)
        {
            var conversation = db.Conversations.Where(x => x.Sid == notification.ConversationSid).Include(x => x.Participants).First();

            //  No conversation find
            if (conversation == null) return;

            var otherParticipant = conversation.Participants.Where(x => x.UserId != null && x.UserId != notification.AuthorId).First();

            //  No other participant
            if (otherParticipant == null) return; 

            var conversationNotification = new ConversationNotification();
            conversationNotification.Body = notification.Body;
            conversationNotification.ConversationId = conversation.Id;
            conversationNotification.UserId = otherParticipant.UserId;
            conversationNotification.NotificationCreator = notification.AuthorId;
            conversationNotification.DateCreated = DateTime.Now;

            db.ConversationNotifications.Add(conversationNotification);
            
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Message received for user {otherParticipant} in conversation {conversation.Id}");
        }
    }
}
