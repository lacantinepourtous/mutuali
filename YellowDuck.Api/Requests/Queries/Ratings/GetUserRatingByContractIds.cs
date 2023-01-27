using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ratings;

namespace YellowDuck.Api.Requests.Queries.Rating
{
    public class GetUserRatingByContractIds : BatchCollectionQuery<GetUserRatingByContractIds.Query, long, UserRating>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetUserRatingByContractIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, UserRating>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.UserRatings
                .Where(x => request.Ids.Contains(x.ContractId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.ContractId);
        }
    }
}
