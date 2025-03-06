using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetAdCertificationsByAdId : BatchCollectionQuery<GetAdCertificationsByAdId.Query, long, AdCertification>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetAdCertificationsByAdId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, AdCertification>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.AdCertifications
                .Where(x => request.Ids.Contains(x.AdId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.AdId);
        }
    }
}
