using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetAdAddressByIds : BatchQuery<GetAdAddressByIds.Query, long, AdAddress>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetAdAddressByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, AdAddress>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.AdAddress
                .Where(x => request.Ids.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
