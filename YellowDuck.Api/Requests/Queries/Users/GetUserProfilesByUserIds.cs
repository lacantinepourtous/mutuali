using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Requests.Queries.Users
{
    public class GetUserProfilesByUserIds : BatchQuery<GetUserProfilesByUserIds.Query, string, UserProfile>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetUserProfilesByUserIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<string, UserProfile>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.UserProfiles
                .Where(x => request.Ids.Contains(x.UserId))
                .ToDictionaryAsync(x => x.UserId, cancellationToken);
        }
    }
}