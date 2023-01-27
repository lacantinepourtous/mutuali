using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Requests.Commands.Mutations.Accounts
{
    public class VerifyEmail : AsyncRequestHandler<VerifyEmail.Input>
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<VerifyEmail> logger;

        public VerifyEmail(UserManager<AppUser> userManager, ILogger<VerifyEmail> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        protected override async Task Handle(Input request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || user.EmailConfirmed) throw new NoNeedToVerifyException();

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await userManager.ConfirmEmailAsync(user, token);
            result.AssertSuccess();

            logger.LogInformation($"Email address verify {user.Email}");
        }

        [MutationInput]
        public class Input : IRequest
        {
            public string Email { get; set; }
        }

        public abstract class VerifyEmailException : RequestValidationException { }
        public class NoNeedToVerifyException : VerifyEmailException { }
    }
}
