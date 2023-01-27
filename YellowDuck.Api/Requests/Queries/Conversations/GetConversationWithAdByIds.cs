using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Conversations;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetConversationWithAdByIds : BatchQuery<GetConversationWithAdByIds.Query, long, Conversation>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetConversationWithAdByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, Conversation>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.Conversations
                .Where(c => request.Ids.Contains(c.Id))
                .Include(x => x.Ad).ThenInclude(x => x.Address)
                .Include(x => x.Ad).ThenInclude(x=> x.Translations)
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
