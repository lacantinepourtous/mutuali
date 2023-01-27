using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Notifications
{
    public class RemoveConversationNotification : IRequestHandler<RemoveConversationNotification.Input, RemoveConversationNotification.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<RemoveConversationNotification> logger;
        private readonly ICurrentUserAccessor currentUserAccessor;

        public RemoveConversationNotification(AppDbContext db, ILogger<RemoveConversationNotification> logger, ICurrentUserAccessor currentUserAccessor)
        {
            this.db = db;
            this.logger = logger;
            this.currentUserAccessor = currentUserAccessor;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var conversation = db.Conversations.Where(x => x.Sid == request.Sid).First();

            if (conversation == null)
            {
                throw new ConversationNotFoundException();
            }

            var notifications = db.ConversationNotifications.Where(x => x.ConversationId == conversation.Id && x.UserId == currentUserAccessor.GetCurrentUserId()).ToList();

            db.ConversationNotifications.RemoveRange(notifications);
            await db.SaveChangesAsync(cancellationToken);

            return new Payload()
            {
                Count = notifications.Count
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public string Sid { get;set; }
        }

        [MutationPayload]
        public class Payload
        {
            public int Count { get; set; }
        }

        public abstract class RemoveConversationNotificationException : RequestValidationException { }

        public class ConversationNotFoundException : RemoveConversationNotificationException { }
    }
}
