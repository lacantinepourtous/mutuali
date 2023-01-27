using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Payment;

namespace YellowDuck.Api.Requests.Queries.Payments
{
    public class GetCheckoutSessionByIds : BatchQuery<GetCheckoutSessionByIds.Query, long, CheckoutSession>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetCheckoutSessionByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, CheckoutSession>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.CheckoutSessions
                .Where(c => request.Ids.Contains(c.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
