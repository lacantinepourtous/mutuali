using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using GraphQL.Conventions;
using System.Threading.Tasks;
using YellowDuck.Api.Authorization;
using System.Collections.Generic;
using YellowDuck.Api.Constants;
using MediatR;
using YellowDuck.Api.Requests.Queries.Users;
using System.Linq;
using YellowDuck.Api.Services.Stripe;
using System;
using YellowDuck.Api.Requests.Queries.Rating;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class UserGraphType : LazyGraphType<AppUser>
    {
        public Id Id { get; }
        public Task<NonNull<string>> Email => WithDataNonNull(x => x.Email);
        public Task<UserType> Type => WithData(x => x.Type);
        public Task<bool> IsConfirmed => WithData(x => x.EmailConfirmed);

        public UserGraphType(IAppUserContext ctx, string userId) : base(() => ctx.LoadUser(userId))
        {
            Id = Id.New<AppUser>(userId);
        }

        public UserGraphType(AppUser user) : base(user)
        {
            Id = user.GetIdentifier();
        }

        public async Task<IProfileGraphType> Profile(IAppUserContext ctx)
        {
            var data = await Data;

            var profile = data.Profile ?? await ctx.LoadProfileByUserId(Id.IdentifierForType<AppUser>());

            return new UserProfileGraphType(profile);
        }

        public async Task<StripeAccountGraphType> StripeAccount(IAppUserContext ctx)
        {
            var data = await Data;

            var account = data.StripeAccount ?? await ctx.LoadStripeAccountByUserId(Id.IdentifierForType<AppUser>());

            return (account != null) ? new StripeAccountGraphType(account) : null;
        }

        [ApplyPolicy(AuthorizationPolicies.InConversation)]
        public async Task<IEnumerable<ConversationGraphType>> Conversations([Inject] IMediator mediator)
        {
            var data = await Data;
            
            var conversations = await mediator.Send(new GetUserConversations.Query
            {
                UserId = data.Id
            });

            return conversations.Select(x => new ConversationGraphType(x)).ToList();
        }

        public async Task<IEnumerable<AdGraphType>> Ads(IAppUserContext ctx)
        {
            var ads = await ctx.LoadAdsByUserId(Id.IdentifierForType<AppUser>());

            return ads.Select(x => new AdGraphType(x)).ToList();
        }

        public async Task<IEnumerable<AlertGraphType>> Alerts(IAppUserContext ctx)
        {
            var alerts = await ctx.LoadAlertsByUserId(Id.IdentifierForType<AppUser>());

            return alerts.Select(x => new AlertGraphType(x)).ToList();
        }

        public async Task<IEnumerable<UserRatingGraphType>> UserRatings(IAppUserContext ctx)
        {
            var userRatings = await ctx.LoadUserRatingByUserId(Id.IdentifierForType<AppUser>());
            return userRatings.Select(x => new UserRatingGraphType(x)).ToList();
        }

        public async Task<double> AverageRating([Inject] IMediator mediator)
        {
            return await mediator.Send(new GetUserAverageRating.Query
            {
                UserId = Id.IdentifierForType<AppUser>()
            });
        }

        public async Task<int> TransactionsCount([Inject] IMediator mediator)
        {
            return await mediator.Send(new GetUserTransactionCount.Query
            {
                UserId = Id.IdentifierForType<AppUser>()
            });
        }

        public Task<DateTime> RegistrationDate() => WithData(x => x.CreatedAtUtc.ToLocalTime());
        public Task<bool> FirstLoginModalClosed => WithData(x => x.FirstLoginModalClosed);
    }
}