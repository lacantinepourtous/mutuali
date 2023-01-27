using YellowDuck.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.Requests.Queries.Users
{
    public class GetUserTransactionCount : IRequestHandler<GetUserTransactionCount.Query, int>
    {
        private readonly AppDbContext db;

        public GetUserTransactionCount(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<int> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.Contracts
                            .Where(x => x.Status == ContractStatus.Closed)
                            .Where(x => x.OwnerId == request.UserId || x.TenantId == request.UserId)
                            .CountAsync(cancellationToken);
        }

        public class Query : IRequest<int>
        {
            public string UserId { get; set; }
        }
    }
}