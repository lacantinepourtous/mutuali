using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Profiles;
using GraphQL.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ads;
using System.Collections.Generic;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.DbModel.Entities.Alerts;

namespace YellowDuck.Api.Gql.Interfaces
{
    public interface IAppUserContext : IUserContext, IDataLoaderContextProvider
    {
        ClaimsPrincipal CurrentUser { get; }
        string CurrentUserId { get; }

        Task<AppUser> LoadUser(string id);
        Task<AppUser> LoadUserWithProfile(string id);
        Task<UserProfile> LoadUserProfile(long id);
        Task<UserProfile> LoadProfileByUserId(string id);
        Task<IEnumerable<UserProfileRegisteringInterest>> LoadRegisteringInterestByUserProfileId(long id);
        Task<IEnumerable<Ad>> LoadAdsByUserId(string id);
        Task<Ad> LoadAd(long id);
        Task<IEnumerable<AdTranslation>> LoadAdTranslations(long id);
        Task<AdAddress> LoadAdAddress(long id);
        Task<IEnumerable<AdGalleryItem>> LoadAdGalleryItems(long id);
        Task<IEnumerable<AdDayAvailability>> LoadDayAvailabilityByAdId(long id);
        Task<IEnumerable<AdEveningAvailability>> LoadEveningAvailabilityByAdId(long id);
        Task<IEnumerable<AdProfessionalKitchenEquipment>> LoadProfessionalKitchenEquipmentsByAdId(long id);
        Task<IEnumerable<AlertProfessionalKitchenEquipment>> LoadProfessionalKitchenEquipmentsByAlertId(long id);
        Task<Alert> LoadAlert(long id);
        Task<IEnumerable<Alert>> LoadAlertsByUserId(string id);
        Task<AlertAddress> LoadAlertAddress(long id);
        Task<Conversation> LoadConversation(long id);
        Task<Conversation> LoadConversationWithAd(long id);
        Task<IEnumerable<ConversationParticipant>> LoadParticipants(long id);
        Task<ConversationParticipant> LoadParticipant(long id);
        Task<Contract> LoadContract(long id);
        Task<IEnumerable<ContractFileItem>> LoadContractFileItems(long id);
        Task<AdRating> LoadAdRating(long id);
        Task<AdRating> LoadAdRatingByContractId(long id);
        Task<IEnumerable<AdRating>> LoadAdRatingByAdId(long id);
        Task<UserRating> LoadUserRating(long id);
        Task<IEnumerable<UserRating>> LoadUserRatingByUserId(string id);
        Task<IEnumerable<UserRating>> LoadUserRatingsByContractId(long id);
        Task<StripeAccount> LoadStripeAccount(long id);
        Task<StripeAccount> LoadStripeAccountByUserId(string id);
        Task<CheckoutSession> LoadCheckoutSession(long id);
        Task<CheckoutSession> LoadCheckoutSessionByContractId(long id);
    }
}