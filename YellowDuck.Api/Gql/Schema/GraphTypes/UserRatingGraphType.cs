using GraphQL.Conventions;
using System;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class UserRatingGraphType : LazyGraphType<UserRating>
    {
        public UserRatingGraphType(IAppUserContext ctx, long id) : base(() => ctx.LoadUserRating(id))
        {
            Id = Id.New<UserRating>(id);
        }

        public UserRatingGraphType(UserRating UserRating) : base(UserRating)
        {
            Id = UserRating.GetIdentifier();
        }

        public Id Id { get; }
        public async Task<UserGraphType> User(IAppUserContext ctx)
        {
            var data = await Data;
            return data.User != null
                ? new UserGraphType(data.User)
                : new UserGraphType(ctx, data.UserId);
        }

        public async Task<UserGraphType> RaterUser(IAppUserContext ctx)
        {
            var data = await Data;
            return data.RaterUser != null
                ? new UserGraphType(data.RaterUser)
                : new UserGraphType(ctx, data.RaterUserId);
        }

        public Task<Rating> RespectRating => WithData(x => x.RespectRating);
        public Task<Rating> CommunicationRating => WithData(x => x.CommunicationRating);
        public Task<Rating> OverallRating => WithData(x => x.OverallRating);

        public Task<DateTime> CreatedAt => WithData(x => x.CreatedAtUtc.ToLocalTime());
        public Task<DateTime> LastUpdatedAt => WithData(x => x.LastUpdatedAtUtc.ToLocalTime());
        public Task<string> Comment => WithData(x => x.Comment);
    }
}
