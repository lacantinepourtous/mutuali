using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Alerts;

namespace YellowDuck.Api.Requests.Queries.Alerts
{
    public class GetAlertsByUserId : BatchCollectionQuery<GetAlertsByUserId.Query, string, Alert>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetAlertsByUserId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<string, Alert>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.Alerts
                .Where(a => request.Ids.Contains(a.UserId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => request.Ids.FirstOrDefault());
        }
    }
}
