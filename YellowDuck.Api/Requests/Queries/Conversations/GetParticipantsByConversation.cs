using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Conversations;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class GetParticipantsByConversation : BatchCollectionQuery<GetParticipantsByConversation.Query, long, ConversationParticipant>
    {
        private readonly AppDbContext db;

        public class Query : BaseQuery { }

        public GetParticipantsByConversation(AppDbContext db)
        {
            this.db = db;
        }

        public override async Task<ILookup<long, ConversationParticipant>> Handle(Query request, CancellationToken cancellationToken)
        {
            var results = await db.ConversationParticipants
                .Where(x => request.Ids.Contains(x.ConversationId))
                .ToListAsync(cancellationToken);

            return results.ToLookup(x => x.ConversationId);
        }
    }
}
