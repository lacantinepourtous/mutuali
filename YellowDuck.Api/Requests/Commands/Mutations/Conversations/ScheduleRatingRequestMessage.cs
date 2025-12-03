using GraphQL.Conventions;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.BackgroundJobs;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Conversations
{
    public class ScheduleRatingRequestMessage : IRequestHandler<ScheduleRatingRequestMessage.Input, ScheduleRatingRequestMessage.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<ScheduleRatingRequestMessage> logger;
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly IBackgroundJobClient backgroundJobClient;

        public ScheduleRatingRequestMessage(
            AppDbContext db, 
            ILogger<ScheduleRatingRequestMessage> logger,
            ICurrentUserAccessor currentUserAccessor,
            IBackgroundJobClient backgroundJobClient)
        {
            this.db = db;
            this.logger = logger;
            this.currentUserAccessor = currentUserAccessor;
            this.backgroundJobClient = backgroundJobClient;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserAccessor.GetCurrentUserId();
            var conversationId = request.ConversationId.LongIdentifierForType<Conversation>();

            var conversation = await db.Conversations.Include(x => x.Participants).FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken) ?? throw new ConversationNotFound();

            if (!conversation.Participants?.Any(x => x.UserId == currentUserId) ?? false)
            {
                throw new UserNotInConversation();
            }

            // Delete existing job if one is scheduled
            if (!string.IsNullOrEmpty(conversation.RatingRequestJobId))
            {
                BackgroundJob.Delete(conversation.RatingRequestJobId);
            }

            // Schedule new job
            var newJobId = backgroundJobClient.Schedule<SendConversationRatingRequestMessage>((x) => x.Run(conversationId), TimeSpan.FromHours(48));

            // Store job ID in database
            conversation.RatingRequestJobId = newJobId;
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"New rating request message scheduled to be sent in 48hrs for conversation {conversationId}");

            return new Payload()
            {
                Conversation = new ConversationGraphType(conversation)
            }; 
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id ConversationId { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public ConversationGraphType Conversation { get; set; }
        }

        public abstract class ScheduleRatingRequestMessageException : RequestValidationException { }

        public class ConversationNotFound : ScheduleRatingRequestMessageException { }
        public class UserNotInConversation : ScheduleRatingRequestMessageException { }
    }
}
