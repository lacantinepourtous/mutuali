using GraphQL.Conventions;
using System;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class AdRatingGraphType : LazyGraphType<AdRating>
    {
        public AdRatingGraphType(IAppUserContext ctx, long id) : base(() => ctx.LoadAdRating(id))
        {
            Id = Id.New<AdRating>(id);
        }

        public AdRatingGraphType(AdRating adRating) : base(adRating)
        {
            Id = adRating.GetIdentifier();
        }

        public Id Id { get; }
        public async Task<AdGraphType> Ad(IAppUserContext ctx)
        {
            var data = await Data;
            return data.Contract != null
               ? new AdGraphType(data.Ad)
               : new AdGraphType(ctx, data.AdId);
        }

        public async Task<ContractGraphType> Contract(IAppUserContext ctx)
        {
            var data = await Data;

            return data.Contract != null
                ? new ContractGraphType(data.Contract)
                : new ContractGraphType(ctx, data.ContractId);
        }

        public async Task<UserGraphType> RaterUser(IAppUserContext ctx)
        {
            var data = await Data;
            return data.RaterUser != null
                ? new UserGraphType(data.RaterUser)
                : new UserGraphType(ctx, data.RaterUserId);
        }
        public Task<Rating> CleanlinessRating => WithData(x => x.CleanlinessRating);
        public Task<Rating> SecurityRating => WithData(x => x.SecurityRating);
        public Task<Rating> ComplianceRating => WithData(x => x.ComplianceRating);

        public Task<DateTime> CreatedAt => WithData(x => x.CreatedAtUtc.ToLocalTime());
    }
}
