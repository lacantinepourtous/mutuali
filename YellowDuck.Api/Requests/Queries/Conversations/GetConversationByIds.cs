using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Conversations;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetConversationByIds : BatchQuery<GetConversationByIds.Query, long, Conversation>
    {
        public class Query : BaseQuery { }

        private readonly AppDbContext db;

        public GetConversationByIds(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<IDictionary<long, Conversation>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.Conversations
                .Where(c => request.Ids.Contains(c.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}
