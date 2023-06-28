using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Alerts;

namespace YellowDuck.Api.Requests.Queries.Alerts
{
    public class GetAlertProfessionalKitchenEquipmentsByAlertId : BatchCollectionQuery<GetAlertProfessionalKitchenEquipmentsByAlertId.Query, long, AlertProfessionalKitchenEquipment>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetAlertProfessionalKitchenEquipmentsByAlertId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, AlertProfessionalKitchenEquipment>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.AlertProfessionalKitchenEquipments
                .Where(x => request.Ids.Contains(x.AlertId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.AlertId);
        }
    } 
}
