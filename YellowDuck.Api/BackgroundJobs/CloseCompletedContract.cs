using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Services.Twilio.Conversations;
using YellowDuck.Api.Services.Twilio.Conversations.Models;

namespace YellowDuck.Api.BackgroundJobs
{
    public class CloseCompletedContract
    {
        private readonly AppDbContext db;
        private readonly IConversationsService conversationsService;
        private readonly ILogger<CloseCompletedContract> logger;

        public CloseCompletedContract(AppDbContext db, IConversationsService conversationsService, ILogger<CloseCompletedContract> logger)
        {
            this.db = db;
            this.conversationsService = conversationsService;
            this.logger = logger;
    }

        public static void RegisterJob(IConfiguration config)
        {
            RecurringJob.AddOrUpdate<CloseCompletedContract>(
                x => x.Run(),
                Cron.Daily(3),
                TimeZoneInfo.FindSystemTimeZoneById(config["systemLocalTimezone"]));
        }

        public async Task Run()
        {
            var today = DateTime.Now;
            var completedContracts = await db.Contracts
                                        .Include(x => x.Conversation)
                                        .Where(x => x.Status == ContractStatus.PayoutDone)
                                        .Where(x => x.EndDate.Date < today.Date)
                                        .ToArrayAsync();

            foreach(var completedContract in completedContracts)
            {
                try
                {
                    // Send rating requestion message in conversation
                    var body = "La location étant terminée, il est temps d'évaluer la transaction.";
                    var attributes = new MessageAttributes
                    {
                        Action = MessageActions.RatingRequest
                    };
                    await conversationsService.SendMessageAsSystem(completedContract.Conversation.Sid, body, attributes);

                    // Set contract status to closed
                    completedContract.Status = ContractStatus.Closed;
                    logger.LogInformation($"Contract status changed to <Closed> ({completedContract.Id})");
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Contract status fail to changed to <Closed> ({completedContract.Id})");
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
