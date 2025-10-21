using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.System;
using YellowDuck.Api.Services.Twilio.Conversations;

namespace YellowDuck.Api.Requests.Commands.Mutations.Conversations
{
    public class CreateConversation : IRequestHandler<CreateConversation.Input, CreateConversation.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<CreateConversation> logger;
        private readonly IConversationsService conversationsService;
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly UserManager<AppUser> userManager;

        public CreateConversation(AppDbContext db, ILogger<CreateConversation> logger, IConversationsService conversationsService, ICurrentUserAccessor currentUserAccessor, UserManager<AppUser> userManager)
        {
            this.db = db;
            this.logger = logger;
            this.conversationsService = conversationsService;
            this.currentUserAccessor = currentUserAccessor;
            this.userManager = userManager;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var owner = (await userManager.GetUsersForClaimAsync(new Claim(AppClaimTypes.AdOwner, request.AdId.ToString()))).LastOrDefault();

            if (owner == null) throw new AdNotFoundException();

            if (owner.Id == currentUserAccessor.GetCurrentUserId())
            {
                throw new CantStartConversationWithSelf();
            }

            var initiator = await currentUserAccessor.GetCurrentUser();
            var conversation = await conversationsService.Create(new AppUser[2] { owner, initiator });

            conversation.AdId = request.AdId.LongIdentifierForType<Ad>();

            db.Conversations.Add(conversation);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"New conversation between owner ({owner.Id}) & initiator ({initiator.Id}) created about the ad ({request.AdId})");

            return new Payload()
            {
                Conversation = new ConversationGraphType(conversation)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id AdId { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public ConversationGraphType Conversation { get; set; }
        }

        public abstract class CreateConversationException : RequestValidationException { }

        public class AdNotFoundException : CreateConversationException { }

        public class CantStartConversationWithSelf : CreateConversationException { }

    }
}
