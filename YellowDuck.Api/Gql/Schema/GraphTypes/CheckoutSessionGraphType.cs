using GraphQL.Conventions;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Services.Stripe;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class CheckoutSessionGraphType : LazyGraphType<CheckoutSession>
    {
        public CheckoutSessionGraphType(IAppUserContext ctx, long id) : base(() => ctx.LoadCheckoutSession(id))
        {
            Id = Id.New<CheckoutSession>(id);
        }

        public CheckoutSessionGraphType(CheckoutSession account) : base(account)
        {
            Id = Id.New<CheckoutSession>(account.Id);
        }

        public async Task<ContractGraphType> Contract(IAppUserContext ctx)
        {
            var data = await Data;
            return data.Contract != null
                ? new ContractGraphType(data.Contract)
                : new ContractGraphType(ctx, data.ContractId);
        }

        public Id Id { get; }

        public async Task<bool> CheckoutSessionComplete([Inject] IPaymentService paymentService)
        {
            var data = await Data;
            var session = await paymentService.GetCheckoutSession(data.CheckoutSessionId);
            return session.PaymentStatus == StripeEnums.PaymentStatus_Paid;
        }

        public async Task<bool> CheckoutSessionCancel([Inject] IPaymentService paymentService)
        {
            var data = await Data;
            var session = await paymentService.GetCheckoutSession(data.CheckoutSessionId);
            return session.PaymentIntent.Status == StripeEnums.PaymentIntentStatus_Canceled;
        }

        public async Task<string> CheckoutLink([Inject] IPaymentService paymentService)
        {
            var data = await Data;
            var checkoutSession = await paymentService.GetCheckoutSession(data.CheckoutSessionId);

            return checkoutSession.Url;
        }
    }
}
