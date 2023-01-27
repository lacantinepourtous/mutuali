using YellowDuck.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Conversations;
using MediatR;

namespace YellowDuck.Api.Requests.Queries.Users
{    
    public class GetUserConversations : IRequestHandler<GetUserConversations.Query, IEnumerable<Conversation>>
    {
        private readonly AppDbContext db;

        public GetUserConversations(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<Conversation>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await db.Conversations.Where(x => x.Participants.Any(c => c.UserId == request.UserId)).ToListAsync();
        }

        public class Query : IRequest<IEnumerable<Conversation>>
        {
            public string UserId { get; set; }
        }
    }
}