using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Contracts;

namespace YellowDuck.Api.Services.Stripe
{
    public interface IPaymentService
    {
        Task<AccountLink> GetAccountOnboardingLink(string stripeAccountId, string redirectUrl);
        Task<AccountLink> GetAccountUpdateLink(string stripeAccountId, string redirectUrl);
        Task<LoginLink> GetAccountDashboardLink(string stripeAccountId);
        Task<Account> CreateExpressAccount(AppUser currentUser);
        Task<Account> GetExpressAccount(string id);
        Task<Session> CreateCheckoutSession(Contract contract);
        Task<Session> GetCheckoutSession(string id);
        Task<bool> CancelCheckoutSession(string id);
        Task<Transfer> CreateTransfer(Contract contract);

        string GetPublicKey();
        string GetSecretKey();
        string GetWebhooksKey();
    }
}
