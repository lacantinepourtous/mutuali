using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ratings;

namespace YellowDuck.Api.Requests.Queries.Rating
{
  public class GetAdRatingsByConversationId : BatchCollectionQuery<GetAdRatingsByConversationId.Query, long, AdRating>
  {
    public class Query : BaseQuery { }

    private readonly AppDbContext db;

    public GetAdRatingsByConversationId(AppDbContext db)
    {
      this.db = db;
    }

    public override async Task<ILookup<long, AdRating>> Handle(Query request, CancellationToken cancellationToken)
    {
      var results = await db.AdRatings
          .Where(x => request.Ids.Contains(x.ConversationId))
          .OrderByDescending(x => x.CreatedAtUtc)
          .ToListAsync(cancellationToken);

      return results.ToLookup(x => x.ConversationId);
    }
  }
}