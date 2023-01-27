using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Profiles;

namespace YellowDuck.Api.Requests.Queries.Users
{
    public class GetRegisteringInterestByUserProfileId : BatchCollectionQuery<GetRegisteringInterestByUserProfileId.Query, long, UserProfileRegisteringInterest>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetRegisteringInterestByUserProfileId(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, UserProfileRegisteringInterest>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.UserProfileRegisteringInterest
                .Where(x => request.Ids.Contains(x.UserProfileId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.UserProfileId);
        }
    }
}
