using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.Requests.Commands.Mutations.Profiles
{
    public class UpdateUserProfile : IRequestHandler<UpdateUserProfile.Input, UpdateUserProfile.Payload>
    {
        protected readonly AppDbContext DbContext;
        private readonly IClock clock;
        private readonly ILogger<UpdateUserProfile> logger;

        public UpdateUserProfile(AppDbContext db, IClock clock, ILogger<UpdateUserProfile> logger)
        {
            DbContext = db;
            this.clock = clock;
            this.logger = logger;
        }

        public async Task<UpdateUserProfile.Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var userId = request.UserId.IdentifierForType<AppUser>();
            var profile = await GetProfileWithUser(userId, cancellationToken);

            if (profile == null)
            {
                var user = await DbContext.Users
                    .Include(x => x.Profile)
                    .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

                if (user == null) throw new UserNotFoundException();

                user.Profile = profile = CreateDefaultProfile(user);
            }

            await UpdateProfileFromRequest(profile, request);

            await DbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"User profile {userId} updated ({typeof(UserProfile).Name})");

            return CreateOutput(profile);
        }

        private UserProfile CreateDefaultProfile(AppUser user)
        {
            return new UserProfile(user.Profile) { User = user };
        }

        private Task<UserProfile> GetProfileWithUser(string userId, CancellationToken cancellationToken)
        {
            return DbContext.UserProfiles
                .OfType<UserProfile>()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        private Task UpdateProfileFromRequest(UserProfile profile, Input request)
        {
            request.FirstName.IfSet(v => profile.FirstName = v.Trim());
            request.LastName.IfSet(v => profile.LastName = v.Trim());
            request.OrganizationName.IfSet(v => profile.OrganizationName = v.Trim());
            request.OrganizationType.IfSet(v => profile.OrganizationType = v);
            request.OrganizationTypeOtherSpecification.IfSet(v => profile.OrganizationTypeOtherSpecification = v.Trim());
            request.Industry.IfSet(v => profile.Industry = v);
            request.IndustryOtherSpecification.IfSet(v => profile.IndustryOtherSpecification = v.Trim());
            request.PostalCode.IfSet(v => profile.PostalCode = v.Trim());
            request.PhoneNumber.IfSet(v => profile.PhoneNumber = v.Trim());
            request.ShowPhoneNumber.IfSet(v => profile.ShowPhoneNumber = v);
            request.ShowEmail.IfSet(v => profile.ShowEmail = v);

            profile.UpdateTimeUtc = clock.GetCurrentInstant().ToDateTimeUtc();

            return Task.CompletedTask;
        }

        private Payload CreateOutput(UserProfile profile)
        {
            return new Payload
            {
                User = new UserGraphType(profile.User)
            };
        }

        public abstract class UpdateProfileException : RequestValidationException { }
        public class UserNotFoundException : UpdateProfileException { }

        [MutationInput]
        public class Input : IRequest<Payload>, IHaveUserId
        {
            public Id UserId { get; set; }

            public Maybe<NonNull<string>> FirstName { get; set; }
            public Maybe<NonNull<string>> LastName { get; set; }
            public Maybe<NonNull<string>> OrganizationName { get; set; }
            public Maybe<OrganizationType> OrganizationType { get; set; }
            public Maybe<NonNull<string>> OrganizationTypeOtherSpecification { get; set; }
            public Maybe<Industry> Industry { get; set; }
            public Maybe<NonNull<string>> IndustryOtherSpecification { get; set; }
            public Maybe<NonNull<string>> PostalCode { get; set; }
            public Maybe<NonNull<string>> PhoneNumber { get; set; }
            public Maybe<bool> ShowPhoneNumber { get; set; }
            public Maybe<bool> ShowEmail { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public UserGraphType User { get; set; }
        }
    }
}
