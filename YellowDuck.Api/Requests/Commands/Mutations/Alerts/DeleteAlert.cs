using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Alerts;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Alerts
{
    public class DeleteAlert : IRequestHandler<DeleteAlert.Input, DeleteAlert.Payload>
    {
        private readonly AppDbContext db;

        public DeleteAlert(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var alertId = request.AlertId.LongIdentifierForType<Alert>();

            var alert = await db.Alerts.FirstOrDefaultAsync(x => x.Id == alertId, cancellationToken);

            if (alert == null) throw new AlertNotFoundException();

            db.Alerts.Remove(alert);
            await db.SaveChangesAsync(cancellationToken);

            return new Payload()
            {
                Success = true
            };
        }

        [MutationInput]
        public class Input : IHaveAlertId, IRequest<Payload>
        {
            public Id AlertId { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public bool Success { get; set; }
        }

        public abstract class DeleteAlertException : RequestValidationException { }

        public class AlertNotFoundException : DeleteAlertException { }
    }
}
