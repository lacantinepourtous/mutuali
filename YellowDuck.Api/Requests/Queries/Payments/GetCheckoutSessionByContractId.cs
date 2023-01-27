using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Payment;

namespace YellowDuck.Api.Requests.Queries.Payments
{
    public class GetCheckoutSessionByContractId : BatchQuery<GetCheckoutSessionByContractId.Query, long, CheckoutSession>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetCheckoutSessionByContractId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, CheckoutSession>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.CheckoutSessions
                .Where(c => request.Ids.Contains(c.ContractId))
                .ToDictionaryAsync(x => x.ContractId, cancellationToken);
        }
    }
}
