using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ratings;

namespace YellowDuck.Api.Requests.Queries.Rating
{
    public class GetUserRatingByIds : BatchQuery<GetUserRatingByIds.Query, long, UserRating>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetUserRatingByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, UserRating>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.UserRatings
                .Where(x => request.Ids.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
