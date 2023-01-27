using YellowDuck.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Payment;

namespace YellowDuck.Api.Requests.Queries.Payments
{
    public class GetStripeAccountByUserIds : BatchQuery<GetStripeAccountByUserIds.Query, string, StripeAccount>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetStripeAccountByUserIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<string, StripeAccount>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.StripeAccounts
                .Where(x => request.Ids.Contains(x.UserId))
                .ToDictionaryAsync(x => x.UserId, cancellationToken);
        }
    }
}