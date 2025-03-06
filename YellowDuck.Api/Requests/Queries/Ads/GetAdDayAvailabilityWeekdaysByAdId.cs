using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetAdAvailabilityRestrictionsByAdId : BatchCollectionQuery<GetAdAvailabilityRestrictionsByAdId.Query, long, AdAvailabilityRestriction>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetAdAvailabilityRestrictionsByAdId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, AdAvailabilityRestriction>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.AdAvailabilityRestrictions
                .Where(x => request.Ids.Contains(x.AdId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.AdId);
        }
    }
}
