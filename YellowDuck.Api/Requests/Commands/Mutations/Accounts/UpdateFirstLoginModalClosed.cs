using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Accounts
{
    public class UpdateFirstLoginModalClosed : IRequestHandler<UpdateFirstLoginModalClosed.Input, UpdateFirstLoginModalClosed.Payload>
    {
        private readonly AppDbContext db;
        private readonly ICurrentUserAccessor currentUserAccessor;

        public UpdateFirstLoginModalClosed(AppDbContext db, ICurrentUserAccessor currentUserAccessor)
        {
            this.db = db;
            this.currentUserAccessor = currentUserAccessor;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUser();

            currentUser.FirstLoginModalClosed = request.FirstLoginModalClosed;

            await db.SaveChangesAsync(cancellationToken);

            return new Payload
            {
                User = new UserGraphType(currentUser)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public bool FirstLoginModalClosed { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public UserGraphType User { get; set; }
        }
    }
}
