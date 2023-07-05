using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Contracts
{
    public class GetAdsByIsAdminOnly : BatchCollectionQuery<GetAdsByIsAdminOnly.Query, bool, Ad>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetAdsByIsAdminOnly(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<bool, Ad>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.Ads
                .Where(a => request.Ids.Contains(a.IsAdminOnly))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => request.Ids.FirstOrDefault());
        }
    }
}
