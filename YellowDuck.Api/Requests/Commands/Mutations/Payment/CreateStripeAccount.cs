using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.Stripe;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Payment
{
    public class CreateStripeAccount : IRequestHandler<CreateStripeAccount.Input, CreateStripeAccount.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<CreateStripeAccount> logger;
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly IPaymentService paymentService;
        private readonly IAppUserContext ctx;

        public CreateStripeAccount(AppDbContext db, ILogger<CreateStripeAccount> logger, IPaymentService paymentService, ICurrentUserAccessor currentUserAccessor, IAppUserContext ctx)
        {
            this.db = db;
            this.logger = logger;
            this.paymentService = paymentService;
            this.currentUserAccessor = currentUserAccessor;
            this.ctx = ctx;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var currentUser = await ctx.LoadUserWithProfile(currentUserAccessor.GetCurrentUserId());

            if (currentUser.StripeAccount != null)
            {
                throw new UserAlreadyHaveStripeAccountException();
            }

            var stripeAccount = await paymentService.CreateExpressAccount(currentUser);

            currentUser.StripeAccount = new StripeAccount()
            {
                StripeAccountId = stripeAccount.Id,
                User = currentUser,
                UserId = currentUser.Id
            };

            db.StripeAccounts.Add(currentUser.StripeAccount);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"New stripe account created for the user ({currentUser.Id})");

            return new Payload()
            {
                Account = new StripeAccountGraphType(currentUser.StripeAccount)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
        }

        [MutationPayload]
        public class Payload
        {
            public StripeAccountGraphType Account { get; set; }
        }

        public abstract class CreateStripeAccountException : RequestValidationException { }

        public class UserAlreadyHaveStripeAccountException : CreateStripeAccountException { }
    }
}
