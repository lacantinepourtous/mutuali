using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Requests.Commands.Mutations.Accounts
{
    public class ChangePassword : IRequestHandler<ChangePassword.Input, ChangePassword.Payload>
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<ChangePassword> logger;

        public ChangePassword(UserManager<AppUser> userManager, ILogger<ChangePassword> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var userId = request.UserId.IdentifierForType<AppUser>();
            var user = await userManager.FindByIdAsync(userId);

            // Fail with InvalidToken error to not leak info about email validity
            if (user == null)
                throw new IdentityResultException(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            result.AssertSuccess();

            logger.LogInformation($"Password changed for user {user.Email}");

            return new Payload
            {
                User = new UserGraphType(user)
            };
        }

        [InputType, MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id UserId { get; set; }
            public NonNull<string> CurrentPassword { get; set; }
            public NonNull<string> NewPassword { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public UserGraphType User { get; set; }
        }
    }
}
