using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;

namespace YellowDuck.Api.Requests.Commands.Mutations.Accounts
{
    public class CompleteAdminRegistration : IRequestHandler<CompleteAdminRegistration.Input, CompleteAdminRegistration.Payload>
    {
        private readonly AppDbContext dbContext;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<CompleteAdminRegistration> logger;

        public CompleteAdminRegistration(AppDbContext dbContext, UserManager<AppUser> userManager, ILogger<CompleteAdminRegistration> logger)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.EmailAddress);
            if (user == null) throw new UserNotFoundException();
            if (user.Type != UserType.Admin) throw new UserNotAdminException();

            var tokenValid = await userManager.VerifyUserTokenAsync(user, TokenProviders.EmailInvites, TokenPurposes.AdminInvite, request.InviteToken);
            if (tokenValid == false) throw new InvalidInviteTokenException();

            var identityResult = await userManager.AddPasswordAsync(user, request.Password);
            identityResult.AssertSuccess();

            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);

            var profile = await GetProfile(user.Id, cancellationToken) ?? (user.Profile = new UserProfile());
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Admin registration completed for {Email}", user.Email);

            return new Payload(new UserGraphType(user));
        }

        private Task<UserProfile?> GetProfile(string userId, CancellationToken cancellationToken)
        {
            return dbContext.UserProfiles
                .OfType<UserProfile>()
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public NonNull<string> EmailAddress { get; set; }
            public NonNull<string> Password { get; set; }
            public NonNull<string> InviteToken { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public Payload(UserGraphType user)
            {
                User = user;
            }

            public NonNull<UserGraphType> User { get; set; }
        }

        public abstract class CompleteAdminRegistrationException : RequestValidationException { }
        public class UserNotFoundException : CompleteAdminRegistrationException { }
        public class UserNotAdminException : CompleteAdminRegistrationException { }
        public class InvalidInviteTokenException : CompleteAdminRegistrationException { }
    }
}
