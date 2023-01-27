using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Services.Mailer;
using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Requests.Commands.Mutations.Accounts
{
    public class SendPasswordReset : AsyncRequestHandler<SendPasswordReset.Input>
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMailer mailer;
        private readonly ILogger<SendPasswordReset> logger;

        public SendPasswordReset(UserManager<AppUser> userManager, IMailer mailer, ILogger<SendPasswordReset> logger)
        {
            this.userManager = userManager;
            this.mailer = mailer;
            this.logger = logger;
        }

        protected override async Task Handle(Input request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null) return;

            logger.LogInformation($"Password reset requested for user {user.Email}");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await mailer.Send(new ResetPasswordEmail(request.Email)
            {
                UserName = user.UserName,
                Token = token
            });
        }

        [MutationInput]
        public class Input : IRequest
        {
            public NonNull<string> Email { get; set; }
        }
    }
}
