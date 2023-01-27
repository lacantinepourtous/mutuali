using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Contracts
{
    public class GetAdsByUserId : BatchCollectionQuery<GetAdsByUserId.Query, string, Ad>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetAdsByUserId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<string, Ad>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.Ads
                .Where(a => request.Ids.Contains(a.UserId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => request.Ids.FirstOrDefault());
        }
    }
}
