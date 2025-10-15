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
            var currentUserId = currentUserAccessor.GetCurrentUserId();
            var userId = request.UserId.IdentifierForType<AppUser>();
            var adId = request.AdId.LongIdentifierForType<Ad>();
            var conversationId = request.ConversationId.LongIdentifierForType<Conversation>();

            // Modifier la note de l'utilisateur si une note existe déjà
            UserRating userRating = null;
            if (request.UserRating != null)
            {
                userRating = await db.UserRatings
                                .FirstOrDefaultAsync(x => x.UserId == userId && x.RaterUserId == currentUserId, cancellationToken) ?? throw new UserRatingDoesntExist();

                userRating.RespectRating = request.UserRating.Respect;
                userRating.CommunicationRating = request.UserRating.Communication;
                userRating.OverallRating = request.UserRating.Overall;
                userRating.Comment = request.UserRating.Comment;
                userRating.LastUpdatedAtUtc = DateTime.UtcNow;
            }

            
            AdRating adRating = null;
            if (request.AdRating != null)
            {
                var existingAdRating = await db.AdRatings
                    .FirstOrDefaultAsync(x => x.AdId == adId && x.RaterUserId == currentUserId, cancellationToken);
                
                if (existingAdRating != null)
                {
                    // Modifier la note de l'annonce si une note existe déjà
                    adRating = existingAdRating;
                    adRating.ComplianceRating = request.AdRating.Compliance;
                    adRating.QualityRating = request.AdRating.Quality;
                    adRating.OverallRating = request.AdRating.Overall;
                    adRating.Comment = request.AdRating.Comment;
                    adRating.LastUpdatedAtUtc = DateTime.UtcNow;
                }
                else
                {
                    // Ou créer une nouvelle note pour l'annonce
                    adRating = new AdRating
                    {
                        AdId = adId,
                        RaterUserId = currentUserId,
                        ConversationId = conversationId,
                        ComplianceRating = request.AdRating.Compliance,
                        QualityRating = request.AdRating.Quality,
                        OverallRating = request.AdRating.Overall,
                        Comment = request.AdRating.Comment,
                        CreatedAtUtc = DateTime.UtcNow,
                        LastUpdatedAtUtc = DateTime.UtcNow
                    };

                    db.AdRatings.Add(adRating);
                }
            }

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Rating of user {userId} updated by user {currentUserId}" + (adRating != null ? $", Ad {adId} rating also updated." : ""));

            return new Payload
            {
                AdRating = new AdRatingGraphType(adRating),
                UserRating = new UserRatingGraphType(userRating)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id UserId { get; set; }
            public Id AdId { get; set; }
            public Id ConversationId { get; set; }
            public UserRatingInput UserRating { get; set; }
            public AdRatingInput AdRating { get; set; }
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
    }
}