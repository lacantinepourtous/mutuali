using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Services.Mailer;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Accounts
{
    public class CreateAdminAccount : IRequestHandler<CreateAdminAccount.Input, CreateAdminAccount.Payload>
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMailer mailer;
        private readonly ILogger<CreateAdminAccount> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CreateAdminAccount(UserManager<AppUser> userManager, IMailer mailer, ILogger<CreateAdminAccount> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.mailer = mailer;
            this.logger = logger;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            var user = new AppUser(request.Email?.Trim())
            {
                Type = UserType.Admin,
                Profile = new UserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName
                },
                AcceptedTos = TosVersion.Latest,
                TosAcceptationDate = DateTime.Now,
                TosAcceptationIpAddress = ipAddress,
                CreatedAtUtc = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user);
            result.AssertSuccess();

            await mailer.Send(new AdminInviteEmail(request.Email)
            {
                FirstName = request.FirstName,
                InviteToken = await userManager.GenerateUserTokenAsync(user, TokenProviders.EmailInvites, TokenPurposes.AdminInvite)
            });

            logger.LogInformation($"Admin account created ({user.Email}).");

            return new Payload
            {
                User = new UserGraphType(user)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public UserGraphType User { get; set; }
        }
    }
}
