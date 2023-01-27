using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Requests.Queries.Users
{
    public class GetUserProfilesByIds : BatchQuery<GetUserProfilesByIds.Query, long, UserProfile>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetUserProfilesByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, UserProfile>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.UserProfiles
                .Where(x => request.Ids.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}