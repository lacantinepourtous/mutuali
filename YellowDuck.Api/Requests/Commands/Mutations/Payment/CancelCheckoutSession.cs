using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.Stripe;

namespace YellowDuck.Api.Requests.Commands.Mutations.Payment
{
    public class CancelCheckoutSession : IRequestHandler<CancelCheckoutSession.Input, CancelCheckoutSession.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<CancelCheckoutSession> logger;
        private readonly IPaymentService paymentService;

        public CancelCheckoutSession(AppDbContext db, ILogger<CancelCheckoutSession> logger, IPaymentService paymentService)
        {
            this.db = db;
            this.logger = logger;
            this.paymentService = paymentService;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var contractId = request.ContractId.LongIdentifierForType<Contract>();
            var checkoutSession = db.CheckoutSessions.Where(x => x.ContractId == contractId)
                .Include(x => x.Contract)
                .FirstOrDefault();

            if (checkoutSession == null)
            {
                throw new CheckoutSessionNotFound();
            }

            var stripeCheckoutSession = await  paymentService.GetCheckoutSession(checkoutSession.CheckoutSessionId);

            if (stripeCheckoutSession.PaymentIntent.Status == StripeEnums.PaymentIntentStatus_Canceled)
            {
                throw new CheckoutSessionAlreadyCancel();
            }

            if (stripeCheckoutSession.PaymentIntent.Status == StripeEnums.PaymentIntentStatus_Succeeded)
            {
                throw new CheckoutSessionAlreadySucceeded();
            }

            var cancelResult = await paymentService.CancelCheckoutSession(checkoutSession.CheckoutSessionId);

            if (cancelResult == false)
            {
                throw new CantCancelStripeCheckoutSession();
            }

            checkoutSession.Contract.CheckoutSession = null;
            db.CheckoutSessions.Remove(checkoutSession);
            await db.SaveChangesAsync(cancellationToken);

            return new Payload()
            {
                CheckoutSession = new CheckoutSessionGraphType(checkoutSession)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id ContractId { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public CheckoutSessionGraphType CheckoutSession { get; set; }
        }

        public abstract class CancelCheckoutSessionException : RequestValidationException { }

        public class CheckoutSessionNotFound : CancelCheckoutSessionException { }

        public class CheckoutSessionAlreadyCancel : CancelCheckoutSessionException { }

        public class CheckoutSessionAlreadySucceeded : CancelCheckoutSessionException { }

        public class CantCancelStripeCheckoutSession : CancelCheckoutSessionException { }
    }
}
