using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ratings
{
    public class DeleteRating : IRequestHandler<DeleteRating.Input, DeleteRating.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<DeleteRating> logger;
        private readonly ICurrentUserAccessor currentUserAccessor;

        public DeleteRating(AppDbContext db, ILogger<DeleteRating> logger, ICurrentUserAccessor currentUserAccessor)
        {
            this.db = db;
            this.logger = logger;
            this.currentUserAccessor = currentUserAccessor;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserAccessor.GetCurrentUserId();

            var hasAd = request.AdRatingId.HasValue;
            var hasUser = request.UserRatingId.HasValue;

            if (!hasAd && !hasUser)
                throw new InvalidOperationException("You must provide a rating ID.");

            if (hasAd && hasUser)
                throw new InvalidOperationException("You must provide only one rating ID.");

            if (hasAd)
            {
                await DeleteRatingAsync(
                    request.AdRatingId!.Value.LongIdentifierForType<AdRating>(),
                    db.AdRatings,
                    x => x.RaterUserId,
                    "AdRating",
                    currentUserId,
                    cancellationToken
                );
            }
            else
            {
                await DeleteRatingAsync(
                    request.UserRatingId!.Value.LongIdentifierForType<UserRating>(),
                    db.UserRatings,
                    x => x.RaterUserId,
                    "UserRating",
                    currentUserId,
                    cancellationToken
                );
            }

            await db.SaveChangesAsync(cancellationToken);

            return new Payload { Success = true };
        }

        private async Task DeleteRatingAsync<T>(
            long id,
            DbSet<T> dbSet,
            Func<T, string> getRaterUserId,
            string label,
            string currentUserId,
            CancellationToken ct
        )
            where T : class
        {
            var rating = await dbSet.FirstOrDefaultAsync(x => EF.Property<long>(x, "Id") == id, ct);

            if (rating == null)
                throw new RatingNotFoundException(id, label);

            var isAdmin = currentUserAccessor.IsUserType(UserType.Admin);
            var ownerId = getRaterUserId(rating);

            if (!isAdmin && ownerId != currentUserId)
                throw new UnauthorizedAccessException("You cannot delete this rating.");

            dbSet.Remove(rating);
            logger.LogInformation($"{label} {id} deleted by user {currentUserId}");
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id? AdRatingId { get; set; }
            public Id? UserRatingId { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public bool Success { get; set; }
        }

        public abstract class DeleteRatingException : Exception
        {
            protected DeleteRatingException(string message) : base(message) { }
        }

        public class RatingNotFoundException : DeleteRatingException
        {
            public RatingNotFoundException(long id, string type)
                : base($"{type} with id {id} was not found.")
            {
            }
        }
    }

}
