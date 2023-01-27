using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetAdEveningAvailabilityWeekdaysByAdId : BatchCollectionQuery<GetAdEveningAvailabilityWeekdaysByAdId.Query, long, AdEveningAvailability>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetAdEveningAvailabilityWeekdaysByAdId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, AdEveningAvailability>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.AdEveningAvailabilityWeekdays
                .Where(x => request.Ids.Contains(x.AdId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.AdId);
        }
    }
}
