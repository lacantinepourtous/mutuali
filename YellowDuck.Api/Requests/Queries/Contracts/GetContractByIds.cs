using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Contracts;

namespace YellowDuck.Api.Requests.Queries.Contracts
{
    public class GetContractByIds : BatchQuery<GetContractByIds.Query, long, Contract>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetContractByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, Contract>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.Contracts
                .Where(c => request.Ids.Contains(c.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
