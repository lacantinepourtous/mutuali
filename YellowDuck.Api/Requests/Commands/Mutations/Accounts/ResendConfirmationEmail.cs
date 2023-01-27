using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.Mailer;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Requests.Commands.Mutations.Accounts
{
    public class ResendConfirmationEmail : AsyncRequestHandler<ResendConfirmationEmail.Input>
    {
        private readonly AppDbContext db;
        private readonly UserManager<AppUser> userManager;
        private readonly IMailer mailer;
        private readonly ILogger<ResendConfirmationEmail> logger;

        public ResendConfirmationEmail(AppDbContext db, UserManager<AppUser> userManager, IMailer mailer, ILogger<ResendConfirmationEmail> logger)
        {
            this.db = db;
            this.userManager = userManager;
            this.mailer = mailer;
            this.logger = logger;
        }

        protected override async Task Handle(Input request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

            if (user == null || user.EmailConfirmed) throw new NoNeedToConfirmException();

            logger.LogInformation($"User {user.Email} requested new email confirmation token.");

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await mailer.Send(new ConfirmEmailEmail(user.Email, token));
        }

        [MutationInput]
        public class Input : IRequest
        {
            public string Email { get; set; }
        }

        public abstract class ResendConfirmationEmailException : RequestValidationException { }
        public class NoNeedToConfirmException : ResendConfirmationEmailException { }
    }
}
