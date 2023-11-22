using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Alerts;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;

namespace YellowDuck.Api.Requests.Commands.Mutations.Alerts
{
    public class UpdateAlert : IRequestHandler<UpdateAlert.Input, UpdateAlert.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<UpdateAlert> logger;

        public UpdateAlert(AppDbContext db, ILogger<UpdateAlert> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var alertId = request.AlertId.LongIdentifierForType<Alert>();

            var alert = await db.Alerts
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == alertId, cancellationToken);

            if (alert == null) throw new AlertNotFoundException();

            request.Category.IfSet(v => alert.Category = v);
            request.Address.IfSet(v => UpdateAddress(alert.Address, v));
            request.Radius.IfSet(v => alert.Radius = v);
            request.ProfessionalKitchenEquipment.IfSet(v => UpdateProfessionalKitchenEquipments(alert, v));
            request.DeliveryTruckType.IfSet(v => alert.DeliveryTruckType = v);
            request.Refrigerated.IfSet(v => alert.Refrigerated = v);
            request.CanSharedRoad.IfSet(v => alert.CanSharedRoad = v);
            request.CanHaveDriver.IfSet(v => alert.CanHaveDriver = v);
            request.DayAvailability.IfSet(v => alert.DayAvailability = v);
            request.EveningAvailability.IfSet(v => alert.EveningAvailability = v);
            request.Email.IfSet(v => alert.Email = v.Trim());

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Alert updated ({alert.Id})");

            return new Payload
            {
                Alert = new AlertGraphType(alert)
            };
        }

        private void ResetAddress(AlertAddress address)
        {
            address.Latitude = 0f;
            address.Longitude = 0f;
            address.Locality = "";
            address.Neighborhood = "";
            address.PostalCode = "";
            address.Raw = "";
            address.Route = "";
            address.StreetNumber = "";
            address.Sublocality = "";
        }

        private void UpdateAddress(AlertAddress address, AddressInput input)
        {
            ResetAddress(address);

            address.Latitude = input.Latitude;
            address.Longitude = input.Longitude;
            address.Raw = input.Raw;

            input.Neighborhood.IfSet(v => address.Neighborhood = v);
            input.Locality.IfSet(v => address.Locality = v);
            input.PostalCode.IfSet(v => address.PostalCode = v);
            input.Route.IfSet(v => address.Route = v);
            input.StreetNumber.IfSet(v => address.StreetNumber = v);
            input.Sublocality.IfSet(v => address.Sublocality = v);
        }

        private void UpdateProfessionalKitchenEquipments(Alert alert, List<ProfessionalKitchenEquipment> professionalKitchenEquipments)
        {
            db.AlertProfessionalKitchenEquipments.RemoveRange(db.AlertProfessionalKitchenEquipments.Where(x => x.AlertId == alert.Id));
            alert.ProfessionalKitchenEquipments = new List<AlertProfessionalKitchenEquipment>();
            professionalKitchenEquipments.ForEach(x => alert.ProfessionalKitchenEquipments.Add(new AlertProfessionalKitchenEquipment() { ProfessionalKitchenEquipment = x }));
        }

        [MutationInput]
        public class Input : IHaveAlertId, IRequest<Payload>
        {
            public Id AlertId { get; set; }
            public Maybe<AdCategory> Category { get; set; }
            public Maybe<bool> DayAvailability { get; set; }
            public Maybe<bool> EveningAvailability { get; set; }
            public Maybe<List<ProfessionalKitchenEquipment>> ProfessionalKitchenEquipment { get; set; }
            public Maybe<DeliveryTruckType> DeliveryTruckType { get; set; }
            public Maybe<bool> Refrigerated { get; set; }
            public Maybe<bool> CanSharedRoad { get; set; }
            public Maybe<bool> CanHaveDriver { get; set; }
            public Maybe<double?> Radius { get; set; }
            public Maybe<NonNull<AddressInput>> Address { get; set; }
            public Maybe<string> Email { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public AlertGraphType Alert { get; set; }
        }

        [InputType]
        public class AddressInput
        {
            public Maybe<NonNull<string>> StreetNumber { get; set; }
            public Maybe<NonNull<string>> Route { get; set; }
            public Maybe<NonNull<string>> Locality { get; set; }
            public NonNull<string> Raw { get; set; }
            public Maybe<NonNull<string>> PostalCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public Maybe<NonNull<string>> Neighborhood { get; set; }
            public Maybe<NonNull<string>> Sublocality { get; set; }
        }


        public abstract class EditAlertException : RequestValidationException { }

        public class AlertNotFoundException : EditAlertException { }
    }
}
