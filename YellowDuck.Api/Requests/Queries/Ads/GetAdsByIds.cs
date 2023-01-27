using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetAdsByIds : BatchQuery<GetAdsByIds.Query, long, Ad>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetAdsByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, Ad>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.Ads
                .Where(c => request.Ids.Contains(c.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
