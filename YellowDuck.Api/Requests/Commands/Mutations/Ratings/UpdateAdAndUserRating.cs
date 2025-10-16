using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ratings
{
    public class UpdateAdAndUserRating : IRequestHandler<UpdateAdAndUserRating.Input, UpdateAdAndUserRating.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<UpdateAdAndUserRating> logger;
        private readonly UserManager<AppUser> userManager;
        private readonly ICurrentUserAccessor currentUserAccessor;

        public UpdateAdAndUserRating(AppDbContext db, ILogger<UpdateAdAndUserRating> logger, UserManager<AppUser> userManager, ICurrentUserAccessor currentUserAccessor)
        {
            this.db = db;
            this.logger = logger;
            this.userManager = userManager;
            this.currentUserAccessor = currentUserAccessor;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request, cancellationToken);

            var currentUserId = currentUserAccessor.GetCurrentUserId();
            var ratedUserId = request.UserId.IdentifierForType<AppUser>();
            var adId = request.AdId.LongIdentifierForType<Ad>();
            var conversationId = request.ConversationId.LongIdentifierForType<Conversation>();

            // Modifier la note de l'utilisateur si une note existe déjà
            var userRating = await db.UserRatings
                            .FirstOrDefaultAsync(x => x.UserId == ratedUserId && x.RaterUserId == currentUserId, cancellationToken) ?? throw new UserRatingDoesntExist();

            userRating.RespectRating = request.UserRating.Respect;
            userRating.CommunicationRating = request.UserRating.Communication;
            userRating.OverallRating = request.UserRating.Overall;
            userRating.Comment = request.UserRating.Comment;
            userRating.LastUpdatedAtUtc = DateTime.UtcNow;
            
            AdRating adRating = null;
            if (request.AdRating.IsSet())
            {
                adRating = await db.AdRatings
                    .FirstOrDefaultAsync(x => x.AdId == adId && x.RaterUserId == currentUserId, cancellationToken);
                
                var isNewAdRating = false;
                if (adRating == null) {
                    adRating = new AdRating();
                    isNewAdRating = true;
                }

                adRating.AdId = adId;
                adRating.RaterUserId = currentUserId;
                adRating.ConversationId = conversationId;
                adRating.ComplianceRating = request.AdRating.Value.Compliance;
                adRating.QualityRating = request.AdRating.Value.Quality;
                adRating.OverallRating = request.AdRating.Value.Overall;
                adRating.Comment = request.AdRating.Value.Comment;
                adRating.LastUpdatedAtUtc = DateTime.UtcNow;

                if (isNewAdRating) {
                    db.AdRatings.Add(adRating);
                }
            }

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Rating of user {ratedUserId} updated by user {currentUserId}" + (adRating != null ? $", Ad {adId} rating also updated." : ""));

            return new Payload
            {
                AdRating = adRating != null ? new AdRatingGraphType(adRating) : null,
                UserRating = new UserRatingGraphType(userRating)
            };
        }

        private async Task ValidateRequest(Input request, CancellationToken cancellationToken)
        {
            if (request.UserRating == null) throw new UserRatingDoesntExist();

            var currentUserId = currentUserAccessor.GetCurrentUserId();
            var ratedUserId = request.UserId.IdentifierForType<AppUser>();
            var adId = request.AdId.LongIdentifierForType<Ad>();
            var conversationId = request.ConversationId.LongIdentifierForType<Conversation>();

            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == currentUserId, cancellationToken) ?? throw new UserDoesntExist();
            var ratedUser = await db.Users.FirstOrDefaultAsync(x => x.Id == ratedUserId, cancellationToken) ?? throw new UserDoesntExist();
            var ad = await db.Ads.FirstOrDefaultAsync(x => x.Id == adId, cancellationToken) ?? throw new AdDoesntExist();
            var conversation = await db.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken) ?? throw new ConversationDoesntExist();
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id UserId { get; set; }
            public Id AdId { get; set; }
            public Id ConversationId { get; set; }
            public UserRatingInput UserRating { get; set; }
            public Maybe<AdRatingInput> AdRating { get; set; }
        }

        [InputType]
        public class AdRatingInput
        {
            public Rating Compliance { get; set; }
            public Rating Quality { get; set; }
            public Rating Overall { get; set; }
            public string Comment { get; set; }
        }

        [InputType]
        public class UserRatingInput
        {
            public Rating Respect { get; set; }
            public Rating Communication { get; set; }
            public Rating Overall { get; set; }
            public string Comment { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public AdRatingGraphType AdRating { get; set; }
            public UserRatingGraphType UserRating { get; set; }
        }

        public abstract class UpdateAdAndUserRatingException : Exception { }
        public class UserRatingDoesntExist : UpdateAdAndUserRatingException { }
        public class AdRatingDoesntExist : UpdateAdAndUserRatingException { }
        public class ConversationDoesntExist : UpdateAdAndUserRatingException { }
        public class UserDoesntExist : UpdateAdAndUserRatingException { }
        public class AdDoesntExist : UpdateAdAndUserRatingException { }
    }
}