using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;

namespace YellowDuck.Api.Requests.Events.Handlers
{
    public class CheckoutSessionExpiredOnPayment : INotificationHandler<CheckoutSessionExpired>
    {
        private readonly ILogger<CheckoutSessionExpiredOnPayment> logger;
        private readonly AppDbContext db;

        public CheckoutSessionExpiredOnPayment(ILogger<CheckoutSessionExpiredOnPayment> logger, AppDbContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        public async Task Handle(CheckoutSessionExpired notification, CancellationToken cancellationToken)
        {
            var checkoutSession = db.CheckoutSessions
                .Where(x => x.CheckoutSessionId == notification.Session.Id)
                .Include(x => x.Contract).ThenInclude(x => x.Conversation)
                .FirstOrDefault();

            if (checkoutSession == null)
            {
                //CheckoutSession is already cancelled
                return;
            }

            var contract = checkoutSession.Contract;

            contract.CheckoutSession = null;
            db.CheckoutSessions.Remove(checkoutSession);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Contract {contract.Id} is cancelled.");
        }
    }
}
