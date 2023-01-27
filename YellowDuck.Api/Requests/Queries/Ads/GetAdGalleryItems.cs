using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetAdGalleryItems : BatchCollectionQuery<GetAdGalleryItems.Query, long, AdGalleryItem>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetAdGalleryItems(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, AdGalleryItem>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.AdGalleryItems
                .Where(x => request.Ids.Contains(x.AdId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.AdId);
        }
    }
}
