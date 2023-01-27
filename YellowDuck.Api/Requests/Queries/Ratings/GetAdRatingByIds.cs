using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ratings;

namespace YellowDuck.Api.Requests.Queries.Rating
{
    public class GetAdRatingByIds : BatchQuery<GetAdRatingByIds.Query, long, AdRating>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetAdRatingByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, AdRating>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.AdRatings
                .Where(x => request.Ids.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
