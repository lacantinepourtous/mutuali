using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetAdsByIds : BatchQuery<GetAdsByIds.Query, long, Ad>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;
        private readonly ICurrentUserAccessor currentUserAccessor;

        public GetAdsByIds(AppDbContext db, ICurrentUserAccessor currentUserAccessor)
        {
            this.db = db;
            this.currentUserAccessor = currentUserAccessor;
        }

        public override async Task<IDictionary<long, Ad>> Handle(Query request, CancellationToken cancellationToken)
        {
            var adsQuery = db.Ads.Where(c => request.Ids.Contains(c.Id));
            if(!currentUserAccessor.IsUserType(UserType.Admin))
            {
                adsQuery = adsQuery.Where(x => x.IsAdminOnly == false);
            }
            return await adsQuery.ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
