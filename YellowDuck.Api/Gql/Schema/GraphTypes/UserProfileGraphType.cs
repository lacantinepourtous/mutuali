using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.Gql.Interfaces;
using GraphQL.Conventions;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Enums;
using System.Collections.Generic;
using System.Linq;
using YellowDuck.Api.Authorization;
using YellowDuck.Api.Constants;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class UserProfileGraphType : LazyGraphType<UserProfile>, IProfileGraphType
    {
        private readonly long id;
        public UserProfileGraphType(IAppUserContext ctx, long id) : base(() => ctx.LoadUserProfile(id))
        {
            this.id = id;
        }

        public UserProfileGraphType(UserProfile profile) : base(profile)
        {
            id = profile.Id;
        }

        public async Task<UserGraphType> User(IAppUserContext ctx)
        {
            var data = await Data;
            return data.User != null
                ? new UserGraphType(data.User)
                : new UserGraphType(ctx, data.UserId);
        }

        public Id Id => Id.New<UserProfile>(id);

        public Task<string> FirstName => WithData(x => x.FirstName);
        public Task<string> LastName => WithData(x => x.LastName);
        public Task<string> PublicName => WithData(x => x.PublicName);

        public Task<string> OrganizationName => WithData(x => x.OrganizationName);

        public Task<string> OrganizationNEQ => WithData(x => x.OrganizationNEQ);

        public Task<OrganizationType> OrganizationType => WithData(x => x.OrganizationType);
        public Task<string> OrganizationTypeOtherSpecification => WithData(x => x.OrganizationTypeOtherSpecification);
        public Task<Industry> Industry => WithData(x => x.Industry);
        public Task<string> IndustryOtherSpecification => WithData(x => x.IndustryOtherSpecification);
        public Task<string> PhoneNumber => WithData(x => x.PhoneNumber);
        public Task<bool> ShowPhoneNumber => WithData(x => x.ShowPhoneNumber);
        public Task<bool> ShowEmail => WithData(x => x.ShowEmail);
        public Task<string> PublicPhoneNumber => WithData(x => (x.ShowPhoneNumber) ? x.PhoneNumber : null);
        public async Task<string> PublicEmail(IAppUserContext ctx)
        {
            var data = await Data;
            var user = data.User ?? await ctx.LoadUser(data.UserId);
            return (data.ShowEmail) ? user.Email : null;
        }
        public Task<bool> ContactAuthorizationNews => WithData(x => x.ContactAuthorizationNews);
        public Task<bool> ContactAuthorizationSurveys => WithData(x => x.ContactAuthorizationSurveys);
        public async Task<IEnumerable<RegisteringInterest>> RegisteringInterest(IAppUserContext ctx)
        {
            var professionalKitchenEquipments = await ctx.LoadRegisteringInterestByUserProfileId(id);
            return professionalKitchenEquipments.Select(x => x.RegisteringInterest).ToList();
        }
        public Task<TosGraphType> AcceptedTos => WithData(x => new TosGraphType(x.User));
    }
}

