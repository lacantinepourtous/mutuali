using GraphQL.Conventions;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Services.Stripe;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class StripeAccountGraphType : LazyGraphType<StripeAccount>
    {
        public StripeAccountGraphType(IAppUserContext ctx, long id) : base(() => ctx.LoadStripeAccount(id))
        {
            Id = Id.New<StripeAccount>(id);
        }

        public StripeAccountGraphType(StripeAccount account) : base(account)
        {
            Id = Id.New<StripeAccount>(account.Id);
        }

        public async Task<UserGraphType> User(IAppUserContext ctx)
        {
            var data = await Data;
            return data.User != null
                ? new UserGraphType(data.User)
                : new UserGraphType(ctx, data.UserId);
        }

        public Id Id { get; }
                
        public async Task<bool> AccountOnboardingComplete([Inject] IPaymentService paymentService)
        {
            var data = await Data;
            var account = await paymentService.GetExpressAccount(data.StripeAccountId);
            return account.ChargesEnabled;
        }

        public async Task<string> AccountOnboardingLink([Inject] IPaymentService paymentService, string redirectUrl)
        {
            var data = await Data;
            var accountLink = await paymentService.GetAccountOnboardingLink(data.StripeAccountId, redirectUrl);
            
            return accountLink.Url;
        }

        public async Task<string> AccountDashboardLink([Inject] IPaymentService paymentService)
        {
            var data = await Data;
            var accountLink = await paymentService.GetAccountDashboardLink(data.StripeAccountId);

            return accountLink.Url;
        }
    }
}
