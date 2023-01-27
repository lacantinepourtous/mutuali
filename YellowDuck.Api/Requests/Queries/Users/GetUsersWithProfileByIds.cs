using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Requests.Queries.Users
{
    public class GetUsersWithProfileByIds : BatchQuery<GetUsersWithProfileByIds.Query, string, AppUser>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetUsersWithProfileByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<string, AppUser>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.Users
                .Where(c => request.Ids.Contains(c.Id))
                .Include(x => x.Profile)
                .Include(x => x.StripeAccount)
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}