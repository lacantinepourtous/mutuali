using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Alerts;

namespace YellowDuck.Api.Requests.Queries.Alerts
{
    public class GetAlertsByIds : BatchQuery<GetAlertsByIds.Query, long, Alert>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetAlertsByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, Alert>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.Alerts
                .Where(c => request.Ids.Contains(c.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
