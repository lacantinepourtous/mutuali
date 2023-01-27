using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.Stripe;

namespace YellowDuck.Api.Requests.Commands.Mutations.Payment
{
    public class CreateCheckoutSession : IRequestHandler<CreateCheckoutSession.Input, CreateCheckoutSession.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<CreateCheckoutSession> logger;
        private readonly IPaymentService paymentService;

        public CreateCheckoutSession(AppDbContext db, ILogger<CreateCheckoutSession> logger, IPaymentService paymentService)
        {
            this.db = db;
            this.logger = logger;
            this.paymentService = paymentService;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var contractId = request.ContractId.LongIdentifierForType<Contract>();
            var contract = db.Contracts.Where(x => x.Id == contractId)
                .Include(x => x.CheckoutSession)
                .FirstOrDefault();

            if (contract == null)
            {
                throw new ContractNotFound();
            }

            if (contract.Status != DbModel.Enums.ContractStatus.Submitted)
            {
                throw new ContractStatusNotSubmitted();
            }

            if (contract.CheckoutSession != null)
            {
                return new Payload()
                {
                    CheckoutSession = new CheckoutSessionGraphType(contract.CheckoutSession)
                };
            }

            var checkoutSession = await paymentService.CreateCheckoutSession(contract);

            contract.CheckoutSession = new CheckoutSession()
            {
                CheckoutSessionId = checkoutSession.Id,
                Contract = contract,
                ContractId = contract.Id
            };

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"New stripe checkout session for the contract ({contract.Id})");

            return new Payload()
            {
                CheckoutSession = new CheckoutSessionGraphType(contract.CheckoutSession)
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

        public abstract class CreateCheckoutSessionException : RequestValidationException { }

        public class ContractNotFound : CreateCheckoutSessionException { }
        
        public class ContractStatusNotSubmitted : CreateCheckoutSessionException { }

        public class ContractAlreadyInCheckout : CreateCheckoutSessionException { }
    }
}
