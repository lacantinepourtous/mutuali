using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetAdProfessionalKitchenEquipmentsByAdId : BatchCollectionQuery<GetAdProfessionalKitchenEquipmentsByAdId.Query, long, AdProfessionalKitchenEquipment>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetAdProfessionalKitchenEquipmentsByAdId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, AdProfessionalKitchenEquipment>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.AdProfessionalKitchenEquipments
                .Where(x => request.Ids.Contains(x.AdId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.AdId);
        }
    }
}
