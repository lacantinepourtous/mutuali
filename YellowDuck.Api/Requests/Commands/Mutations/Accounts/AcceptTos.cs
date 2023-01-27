using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Accounts
{
    public class AcceptTos : IRequestHandler<AcceptTos.Input, AcceptTos.Payload>
    {
        private readonly AppDbContext db;
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AcceptTos(AppDbContext db, ICurrentUserAccessor currentUserAccessor, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.currentUserAccessor = currentUserAccessor;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var currentUser = await currentUserAccessor.GetCurrentUser();
            if (currentUser.AcceptedTos >= request.TosVersion)
                throw new VersionAlreadyAcceptedException();

            var ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            if (currentUser.AcceptedTos < request.TosVersion)
            {
                currentUser.AcceptedTos = request.TosVersion;
                currentUser.TosAcceptationDate = DateTime.Now;
                currentUser.TosAcceptationIpAddress = ipAddress;
            }

            await db.SaveChangesAsync(cancellationToken);

            return new Payload
            {
                User = new UserGraphType(currentUser)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public TosVersion TosVersion { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public UserGraphType User { get; set; }
        }


        public class VersionAlreadyAcceptedException : RequestValidationException { }
    }
}
