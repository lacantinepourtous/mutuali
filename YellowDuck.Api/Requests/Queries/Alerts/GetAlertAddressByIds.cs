using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Alerts;

namespace YellowDuck.Api.Requests.Queries.Alerts
{
    public class GetAlertAddressByIds : BatchQuery<GetAlertAddressByIds.Query, long, AlertAddress>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetAlertAddressByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, AlertAddress>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.AlertAddress
                .Where(x => request.Ids.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
