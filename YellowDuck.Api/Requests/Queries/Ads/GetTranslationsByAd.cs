using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetTranslationsByAd : BatchCollectionQuery<GetTranslationsByAd.Query, long, AdTranslation>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetTranslationsByAd(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, AdTranslation>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.AdTranslations
                .Where(x => request.Ids.Contains(x.AdId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.AdId);
        }
    }
}
