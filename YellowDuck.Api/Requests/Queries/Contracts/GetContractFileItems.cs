using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Contracts;

namespace YellowDuck.Api.Requests.Queries.Contracts
{
    public class GetContractFileItems : BatchCollectionQuery<GetContractFileItems.Query, long, ContractFileItem>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetContractFileItems(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, ContractFileItem>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.ContractFiles
                .Where(x => request.Ids.Contains(x.ContractId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.ContractId);
        }
    }
}
