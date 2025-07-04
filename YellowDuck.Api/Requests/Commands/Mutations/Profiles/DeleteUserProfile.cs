using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;

namespace YellowDuck.Api.Requests.Commands.Mutations.Profiles
{
    public class DeleteUserProfile : IRequestHandler<DeleteUserProfile.Input, DeleteUserProfile.Payload>
    {
        private readonly AppDbContext db;

        public DeleteUserProfile(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var userId = request.UserId.IdentifierForType<AppUser>();

            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null) throw new UserNotFoundException();

            db.Users.Remove(user);
            // db.UserProfiles.Remove(userProfile);

            await db.SaveChangesAsync(cancellationToken);

            return new Payload()
            {
                Success = true
            };
        }

        [MutationInput]
        public class Input : IHaveUserId, IRequest<Payload>
        {
            public Id UserId { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public bool Success { get; set; }
        }

        public abstract class DeleteUserProfileException : RequestValidationException { }
        public class UserNotFoundException : DeleteUserProfileException { }
    }
}
