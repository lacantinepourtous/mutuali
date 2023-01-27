using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Payment;

namespace YellowDuck.Api.Requests.Queries.Payments
{
    public class GetStripeAccountByIds : BatchQuery<GetStripeAccountByIds.Query, long, StripeAccount>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetStripeAccountByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, StripeAccount>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.StripeAccounts
                .Where(c => request.Ids.Contains(c.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
