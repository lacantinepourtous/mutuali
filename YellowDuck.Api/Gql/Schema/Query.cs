using YellowDuck.Api.Authorization;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.Enums;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.Extensions;
using System.Collections.Generic;
using System.Linq;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.Services.Twilio;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Utilities;
using MediatR;
using YellowDuck.Api.Requests.Queries.Users;
using YellowDuck.Api.Requests.Queries.Ads;
using YellowDuck.Api.DbModel.Entities.Alerts;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Gql.Schema
{
    public class Query
    {
        [ApplyPolicy(AuthorizationPolicies.LoggedIn)]
        [Description("The currently authenticated user.")]
        public async Task<UserGraphType> Me(IAppUserContext ctx)
        {
            var user = await ctx.LoadUser(ctx.CurrentUserId);
            return user != null
                ? new UserGraphType(user)
                : null;
        }

        [Description("Complete user profile of a user, identified by it's ID.")]
        public async Task<UserProfileGraphType> UserProfile(IAppUserContext ctx, Id id)
        {
            var userProfile = await ctx.LoadUserProfile(id.LongIdentifierForType<UserProfile>());
            return new UserProfileGraphType(userProfile);
        }

        [ApplyPolicy(AuthorizationPolicies.IsUser)]
        public async Task<string> MyTwilioToken(IAppUserContext ctx, [Inject] ITwilioService twilioService)
        {
            var user = await ctx.LoadUser(ctx.CurrentUserId);
            return user != null
                ? twilioService.GetAppUserToken(user)
                : null;
        }

        [ApplyPolicy(AuthorizationPolicies.IsAdmin)]
        public async Task<UserGraphType> User(
            [Inject] AppDbContext db,
            Id id)
        {
            var user = await db.Users.FindAsync(id.IdentifierForType<AppUser>());
            if (user == null) return null;

            return new UserGraphType(user);
        }

        [ApplyPolicy(AuthorizationPolicies.IsAdmin)]
        [Description("All users")]
        public async Task<Pagination<UserGraphType>> Users(
            [Inject] IMediator mediator,
            int page, int limit)
        {
            var results = await mediator.Send(new SearchUsers.Query
            {
                Page = new Page(page, limit),
            });

            return results.Map(x => new UserGraphType(x));
        }

        [ApplyPolicy(AuthorizationPolicies.IsAdmin)]
        public async Task<UserGraphType> UserByEmail(
            [Inject] AppDbContext db,
            string email)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return null;

            return new UserGraphType(user);
        }

        [Description("Details about a specific ad, identified by it's ID.")]
        public async Task<AdGraphType> Ad(IAppUserContext ctx, Id id)
        {
            var ad = await ctx.LoadAd(id.LongIdentifierForType<Ad>());
            if(ad == null) return null;
            return new AdGraphType(ad);
        }

        [Description("Details about all the active ads")]
        public async Task<IEnumerable<AdGraphType>> Ads(
            [Inject] IAppCache cache, 
            [Inject] IMediator mediator,
[Inject] ICurrentUserAccessor currentUserAccessor,
            AdCategory? category = null, 
            IList<DayOfWeek> dayAvailability = null,
            IList<DayOfWeek> eveningAvailability = null, 
            IList<ProfessionalKitchenEquipment> professionalKitchenEquipment = null,
            DeliveryTruckType? deliveryTruckType = null, 
            bool? refrigerated = null, 
            bool? canSharedRoad = null, 
            bool? canHaveDriver = null)
        {
            var adsQuery = new SearchAds.Query
            {
                Category = category,
                DayAvailability = dayAvailability,
                EveningAvailability = eveningAvailability,
                ProfessionalKitchenEquipment = professionalKitchenEquipment,
                DeliveryTruckType = deliveryTruckType,
                Refrigerated = refrigerated,
                CanHaveDriver = canHaveDriver,
                CanSharedRoad = canSharedRoad,
            };

            // No cache for admin + show admin only ads
            if (currentUserAccessor.IsUserType(UserType.Admin)) {
                adsQuery.ShowAdminOnly = true;
                var ads = await mediator.Send(adsQuery);
                return ads.Select(x => new AdGraphType(x));
            }

            return await cache.GetOrAddAsync($"Ads:{category}-{string.Join(",", dayAvailability.OrderBy(x => x))}-{string.Join(",", eveningAvailability.OrderBy(x => x))}-{string.Join(",", professionalKitchenEquipment.OrderBy(x => x))}-{deliveryTruckType}-{refrigerated}-{canHaveDriver}-{canSharedRoad}", async entry =>
            {
                entry.SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddMinutes(5));
                entry.Priority = CacheItemPriority.Low;

                var ads = await mediator.Send(adsQuery);
                return ads.Select(x => new AdGraphType(x));
            });
        }

        [Description("Details about a specific alert, identified by it's ID.")]
        public async Task<AlertGraphType> Alert(IAppUserContext ctx, Id id)
        {
            var alert = await ctx.LoadAlert(id.LongIdentifierForType<Alert>());
            return new AlertGraphType(alert);
        }

        [Description("Details about a specific conversation, identified by it's ID.")]
        public async Task<ConversationGraphType> Conversation(IAppUserContext ctx, Id id)
        {
            var conversation = await ctx.LoadConversation(id.LongIdentifierForType<Conversation>());
            return new ConversationGraphType(conversation);
        }

        [Description("Details about a specific contract, identified by it's ID.")]
        public async Task<ContractGraphType> Contract(IAppUserContext ctx, Id id)
        {
            var conversation = await ctx.LoadContract(id.LongIdentifierForType<Contract>());
            return new ContractGraphType(conversation);
        }

        public async Task<VerifyTokenPayload> VerifyToken(
            [Inject] UserManager<AppUser> userManager,
            [Inject] IOptions<IdentityOptions> identityOptions,
            string email, string token, TokenType type)
        {
            var user = await userManager.FindByEmailAsync(email);
            var status = TokenStatus.Invalid;

            if (user != null)
            {
                var (provider, purpose) = GetTokenProviderAndPurpose();
                if (await userManager.VerifyUserTokenAsync(user, provider, purpose, token))
                    status = TokenStatus.Valid;
            }

            if (status == TokenStatus.Invalid)
            {
                if (type == TokenType.ConfirmEmail)
                {
                    if (user?.EmailConfirmed == true)
                    {
                        status = TokenStatus.UserConfirmed;
                    }
                }
            }

            return new VerifyTokenPayload
            {
                Status = status,
                User = status == TokenStatus.Valid ? new UserGraphType(user) : null
            };

            (string provider, string purpose) GetTokenProviderAndPurpose()
            {
                switch (type)
                {
                    case TokenType.ResetPassword:
                        return (identityOptions.Value.Tokens.PasswordResetTokenProvider, "ResetPassword");
                    case TokenType.ConfirmEmail:
                        return (identityOptions.Value.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation");
                    case TokenType.AdminInvitation:
                        return (TokenProviders.EmailInvites, TokenPurposes.AdminInvite);
                    default: throw new ArgumentOutOfRangeException(nameof(type));
                }
            }
        }
    }
}