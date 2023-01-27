using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Services.Stripe;
using YellowDuck.Api.Services.Twilio.Conversations;

namespace YellowDuck.Api.Requests.Events.Handlers
{
    public class CompleteSessionOnPayment : INotificationHandler<CompleteSession>
    {
        private readonly IConversationsService conversationsService;
        private readonly ILogger<CompleteSessionOnPayment> logger;
        private readonly AppDbContext db;

        public CompleteSessionOnPayment(IConversationsService conversationsService, ILogger<CompleteSessionOnPayment> logger, AppDbContext db)
        {
            this.conversationsService = conversationsService;
            this.logger = logger;
            this.db = db;
        }

        // TODO :: Translation
        public async Task Handle(CompleteSession notification, CancellationToken cancellationToken)
        {
            var checkoutSession = db.CheckoutSessions
                .Where(x => x.CheckoutSessionId == notification.Session.Id)
                .Include(x => x.Contract).ThenInclude(x => x.Conversation)
                .FirstOrDefault();

            var contract = checkoutSession.Contract;
            
            if (notification.Session.PaymentStatus == StripeEnums.PaymentStatus_Paid)
            {
                contract.Status = ContractStatus.Paid;
            }

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Contract {contract.Id} is fully payed.");

            var body = "Tout est en règle. Le contrat est accepté et le paiement sera versé au propriétaire à la date concernée par la réservation.";
            await conversationsService.SendMessageAsSystem(contract.Conversation.Sid, body);
        }
    }
}
