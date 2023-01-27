using GraphQL.Conventions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.Gql.Interfaces;

namespace YellowDuck.Api.Services.Stripe
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAppUserContext ctx;
        private readonly UserManager<AppUser> userManager;

        public PaymentService(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IAppUserContext ctx)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            this.ctx = ctx;
            this.userManager = userManager;
        }

        public async Task<Account> CreateExpressAccount(AppUser currentUser)
        {
            StripeConfiguration.ApiKey = GetSecretKey();

            var options = new AccountCreateOptions
            {
                Type = "express",
                Country = "CA",
                Capabilities = new AccountCapabilitiesOptions
                {
                    Transfers = new AccountCapabilitiesTransfersOptions
                    {
                        Requested = true,
                    }
                },
                Email = currentUser.Email,
                BusinessProfile = new AccountBusinessProfileOptions()
                {
                    Name = currentUser.Profile.OrganizationName
                }
            };

            var service = new AccountService();
            return await service.CreateAsync(options);
        }

        // TODO :: Translation
        public async Task<Session> CreateCheckoutSession(Contract contract)
        {
            StripeConfiguration.ApiKey = GetSecretKey();

            var request = httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            Conversation conversation = await ctx.LoadConversationWithAd(contract.ConversationId);
            AppUser tenant = await ctx.LoadUser(contract.TenantId);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                CustomerEmail = tenant.Email,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Name = $"Location MutuAli - {conversation.Ad.Translations[0].Title}",
                        Amount = Convert.ToInt64(contract.Price * 100), // Mutiple by 100 because Stripe need a round number
                        Currency = "cad",
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = $"{baseUrl}/payment-success?contractId={Id.New<Contract>(contract.Id)}",
                CancelUrl = $"{baseUrl}/payment-cancel?contractId={Id.New<Contract>(contract.Id)}",
                PaymentIntentData = new SessionPaymentIntentDataOptions()
                {
                    TransferGroup = Id.New<Contract>(contract.Id).ToString()
                },
                Locale = "fr-CA",
                ExpiresAt = DateTime.Now.AddMinutes(60)
            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }

        public async Task<bool> CancelCheckoutSession(string id)
        {
            StripeConfiguration.ApiKey = GetSecretKey();
            var service = new SessionService();
            var session = await service.GetAsync(id);

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CancelAsync(session.PaymentIntentId, new PaymentIntentCancelOptions()
            {
                CancellationReason = StripeEnums.CancellationReason_Abandoned
            });

            return paymentIntent.Status == StripeEnums.PaymentIntentStatus_Canceled;
        }

        public async Task<Transfer> CreateTransfer(Contract contract)
        {
            var session = await GetCheckoutSession(contract.CheckoutSession.CheckoutSessionId);
            var charge = session.PaymentIntent.Charges.First();

            StripeAccount stripeAccount = await ctx.LoadStripeAccountByUserId(contract.OwnerId);

            var options = new TransferCreateOptions
            {
                Amount = Convert.ToInt64(contract.Price * 100), // Mutiple by 100 because Stripe need a round number,
                Currency = "cad",
                SourceTransaction = charge.Id,
                Destination = stripeAccount.StripeAccountId,
                TransferGroup = Id.New<Contract>(contract.Id).ToString()
            };

            var service = new TransferService();
            return await service.CreateAsync(options);
        }

        public Task<Account> GetExpressAccount(string id)
        {
            StripeConfiguration.ApiKey = GetSecretKey();
            var service = new AccountService();
            return service.GetAsync(id);
        }

        public Task<Session> GetCheckoutSession(string id)
        {
            StripeConfiguration.ApiKey = GetSecretKey();
            var service = new SessionService();
            return service.GetAsync(id, new SessionGetOptions()
            {
                Expand = new List<string>() { "payment_intent" }
            });
        }

        public Task<AccountLink> GetAccountOnboardingLink(string stripeAccountId, string redirectUrl)
        {
            return GetAccountLink(stripeAccountId, "account_onboarding", redirectUrl);
        }

        public Task<AccountLink> GetAccountUpdateLink(string stripeAccountId, string redirectUrl)
        {
            return GetAccountLink(stripeAccountId, "account_update", redirectUrl);
        }

        public Task<LoginLink> GetAccountDashboardLink(string stripeAccountId)
        {
            StripeConfiguration.ApiKey = GetSecretKey();
            var service = new LoginLinkService();
            return service.CreateAsync(stripeAccountId);
        }

        private Task<AccountLink> GetAccountLink(string stripeAccountId, string linkType, string redirectUrl)
        {
            StripeConfiguration.ApiKey = GetSecretKey();

            var request = httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var options = new AccountLinkCreateOptions
            {
                Account = stripeAccountId,
                RefreshUrl = $"{baseUrl}/stripe-connect-refresh?redirectUrl={redirectUrl}",
                ReturnUrl = $"{baseUrl}/stripe-connect-validate?redirectUrl={redirectUrl}",
                Type = linkType
            };

            var service = new AccountLinkService();
            return service.CreateAsync(options);
        }

        public string GetPublicKey()
        {
            return configuration.GetValue<string>("stripe:publicKey");
        }

        public string GetSecretKey()
        {
            return configuration.GetValue<string>("stripe:secretKey");
        }

        public string GetWebhooksKey()
        {
            return configuration.GetValue<string>("stripe:webHookKey");
        }
    }
}
