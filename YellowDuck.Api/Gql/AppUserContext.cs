using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Requests.Queries.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.Requests.Queries.Ads;
using System.Collections.Generic;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.Requests.Queries.Contracts;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.Requests.Queries.Payments;
using YellowDuck.Api.Requests.Queries.Rating;
using YellowDuck.Api.Requests.Queries.Alerts;
using YellowDuck.Api.DbModel.Entities.Alerts;

namespace YellowDuck.Api.Gql
{
    public class AppUserContext : IAppUserContext
    {
        private readonly IServiceProvider services;
        private readonly DataLoader loader;

        private TService Get<TService>() => services.GetRequiredService<TService>();

        public ClaimsPrincipal CurrentUser => Get<IHttpContextAccessor>().HttpContext?.User;
        public string CurrentUserId => CurrentUser.GetUserId();

        public AppUserContext(IServiceProvider services, DataLoader loader)
        {
            this.services = services;
            this.loader = loader;
        }

        public async Task FetchData(CancellationToken token)
        {
            await loader.DispatchAllAsync(token);
        }

        public Task<AppUser> LoadUser(string id) =>
            loader.LoadOne<GetUsersByIds.Query, AppUser, string>(id);

        public Task<AppUser> LoadUserWithProfile(string id) =>
            loader.LoadOne<GetUsersWithProfileByIds.Query, AppUser, string>(id);

        public Task<UserProfile> LoadUserProfile(long id) =>
            loader.LoadOne<GetUserProfilesByIds.Query, UserProfile, long>(id);

        public Task<UserProfile> LoadProfileByUserId(string id) =>
            loader.LoadOne<GetUserProfilesByUserIds.Query, UserProfile, string>(id);

        public Task<IEnumerable<UserProfileRegisteringInterest>> LoadRegisteringInterestByUserProfileId(long id) =>
          loader.LoadCollection<GetRegisteringInterestByUserProfileId.Query, UserProfileRegisteringInterest, long>(id);

        public Task<Ad> LoadAd(long id) =>
            loader.LoadOne<GetAdsByIds.Query, Ad, long>(id);

        public Task<IEnumerable<Ad>> LoadAdsByUserId(string id) =>
            loader.LoadCollection<GetAdsByUserId.Query, Ad, string>(id);

        public Task<IEnumerable<Ad>> LoadAdByIsAdminOnly(bool isAdminOnly) =>
            loader.LoadCollection<GetAdsByIsAdminOnly.Query, Ad, bool>(isAdminOnly);

        public Task<IEnumerable<AdTranslation>> LoadAdTranslations(long id) =>
            loader.LoadCollection<GetTranslationsByAd.Query, AdTranslation, long>(id);

        public Task<AdAddress> LoadAdAddress(long id) =>
            loader.LoadOne<GetAdAddressByIds.Query, AdAddress, long>(id);

        public Task<IEnumerable<AdGalleryItem>> LoadAdGalleryItems(long id) =>
            loader.LoadCollection<GetAdGalleryItems.Query, AdGalleryItem, long>(id);

        public Task<IEnumerable<AdDayAvailability>> LoadDayAvailabilityByAdId(long id) =>
            loader.LoadCollection<GetAdDayAvailabilityWeekdaysByAdId.Query, AdDayAvailability, long>(id);

        public Task<IEnumerable<AdEveningAvailability>> LoadEveningAvailabilityByAdId(long id) =>
           loader.LoadCollection<GetAdEveningAvailabilityWeekdaysByAdId.Query, AdEveningAvailability, long>(id);

        public Task<IEnumerable<AdAvailabilityRestriction>> LoadAvailabilityRestrictionsByAdId(long id) =>
           loader.LoadCollection<GetAdAvailabilityRestrictionsByAdId.Query, AdAvailabilityRestriction, long>(id);

        public Task<IEnumerable<AdCertification>> LoadCertificationsByAdId(long id) =>
            loader.LoadCollection<GetAdCertificationsByAdId.Query, AdCertification, long>(id);

        public Task<IEnumerable<AdAllergen>> LoadAllergensByAdId(long id) =>
         loader.LoadCollection<GetAdAllergensByAdId.Query, AdAllergen, long>(id);

        public Task<IEnumerable<AdProfessionalKitchenEquipment>> LoadProfessionalKitchenEquipmentsByAdId(long id) =>
          loader.LoadCollection<GetAdProfessionalKitchenEquipmentsByAdId.Query, AdProfessionalKitchenEquipment, long>(id);

        public Task<IEnumerable<AlertProfessionalKitchenEquipment>> LoadProfessionalKitchenEquipmentsByAlertId(long id) =>
          loader.LoadCollection<GetAlertProfessionalKitchenEquipmentsByAlertId.Query, AlertProfessionalKitchenEquipment, long>(id);

        public Task<Alert> LoadAlert(long id) =>
            loader.LoadOne<GetAlertsByIds.Query, Alert, long>(id);

        public Task<IEnumerable<Alert>> LoadAlertsByUserId(string id) =>
            loader.LoadCollection<GetAlertsByUserId.Query, Alert, string>(id);

        public Task<AlertAddress> LoadAlertAddress(long id) =>
            loader.LoadOne<GetAlertAddressByIds.Query, AlertAddress, long>(id);

        public Task<Conversation> LoadConversation(long id) =>
            loader.LoadOne<GetConversationByIds.Query, Conversation, long>(id);

        public Task<Conversation> LoadConversationWithAd(long id) =>
            loader.LoadOne<GetConversationWithAdByIds.Query, Conversation, long>(id);

        public Task<IEnumerable<ConversationParticipant>> LoadParticipants(long id) =>
            loader.LoadCollection<GetParticipantsByConversation.Query, ConversationParticipant, long>(id);

        public Task<ConversationParticipant> LoadParticipant(long id) =>
            loader.LoadOne<GetConversationParticipantByIds.Query, ConversationParticipant, long>(id);

        public Task<Contract> LoadContract(long id) =>
            loader.LoadOne<GetContractByIds.Query, Contract, long>(id);

        public Task<IEnumerable<ContractFileItem>> LoadContractFileItems(long id) =>
            loader.LoadCollection<GetContractFileItems.Query, ContractFileItem, long>(id);

        public Task<AdRating> LoadAdRating(long id) =>
            loader.LoadOne<GetAdRatingByIds.Query, AdRating, long>(id);

        public Task<IEnumerable<AdRating>> LoadAdRatingByAdId(long id) =>
            loader.LoadCollection<GetAdRatingsByAdId.Query, AdRating, long>(id);

        public Task<IEnumerable<AdRating>> LoadAdRatingByConversationId(long id) =>
            loader.LoadCollection<GetAdRatingsByConversationId.Query, AdRating, long>(id);

        public Task<UserRating> LoadUserRating(long id) =>
            loader.LoadOne<GetUserRatingByIds.Query, UserRating, long>(id);

        public Task<IEnumerable<UserRating>> LoadUserRatingByUserId(string id) =>
            loader.LoadCollection<GetUserRatingsByUserId.Query, UserRating, string>(id);

        public Task<IEnumerable<UserRating>> LoadUserRatingByConversationId(long id) =>
            loader.LoadCollection<GetUserRatingsByConversationId.Query, UserRating, long>(id);

        public Task<StripeAccount> LoadStripeAccount(long id) =>
            loader.LoadOne<GetStripeAccountByIds.Query, StripeAccount, long>(id);

        public Task<StripeAccount> LoadStripeAccountByUserId(string id) =>
            loader.LoadOne<GetStripeAccountByUserIds.Query, StripeAccount, string>(id);

        public Task<CheckoutSession> LoadCheckoutSession(long id) =>
            loader.LoadOne<GetCheckoutSessionByIds.Query, CheckoutSession, long>(id);

        public Task<CheckoutSession> LoadCheckoutSessionByContractId(long id) =>
            loader.LoadOne<GetCheckoutSessionByContractId.Query, CheckoutSession, long>(id);
    }
}