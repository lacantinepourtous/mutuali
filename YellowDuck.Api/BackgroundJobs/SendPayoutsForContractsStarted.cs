using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Services.Stripe;
using YellowDuck.Api.Services.Twilio.Conversations;

namespace YellowDuck.Api.BackgroundJobs
{
    public class SendPayoutsForContractsStarted
    {
        private readonly AppDbContext db;
        private readonly ILogger<SendPayoutsForContractsStarted> logger;
        private readonly IPaymentService paymentService;
        private readonly IConversationsService conversationsService;

        public SendPayoutsForContractsStarted(AppDbContext db, IPaymentService paymentService, IConversationsService conversationsService, ILogger<SendPayoutsForContractsStarted> logger)
        {
            this.db = db;
            this.paymentService = paymentService;
            this.logger = logger;
            this.conversationsService = conversationsService;
        }

        public static void RegisterJob(IConfiguration config)
        {
            RecurringJob.AddOrUpdate<SendPayoutsForContractsStarted>(
                x => x.Run(),
                Cron.Daily(1, 30),
                TimeZoneInfo.FindSystemTimeZoneById(config["systemLocalTimezone"]));
        }

        public async Task Run()
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var contracts = db.Contracts
                .Where(x => x.Status == ContractStatus.Paid && x.StartDate <= tomorrow.Date)
                .Include(x => x.CheckoutSession)
                .Include(x => x.Conversation)
                .ToArray();

            foreach (var contract in contracts)
            {
                var transfer = await paymentService.CreateTransfer(contract);

                contract.Payout = new Payout()
                {
                    Contract = contract,
                    ContractId = contract.Id,
                    TransferId = transfer.Id
                };
                contract.Status = ContractStatus.PayoutDone;

                await db.SaveChangesAsync();

                logger.LogInformation($"New stripe transfer for the contract ({contract.Id})");

                var body = $"Un paiement de {contract.Price.ToString("N2")} $ CA est en route vers le compte en banque du propriétaire. Un délai maximum de sept jours est attendu. Si suite à ce délai l'argent n'est toujours pas déposé, veuillez entrer en communication à l'adresse suivante info@mutuali.ca";
                await conversationsService.SendMessageAsSystem(contract.Conversation.Sid, body);
            }
        }
    }
}
