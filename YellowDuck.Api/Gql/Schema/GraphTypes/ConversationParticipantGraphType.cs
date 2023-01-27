using GraphQL.Conventions;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.Gql.Interfaces;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class ConversationParticipantGraphType : LazyGraphType<ConversationParticipant>
    {
        private readonly long id;

        public ConversationParticipantGraphType(IAppUserContext ctx, long participantId) : base(() => ctx.LoadParticipant(participantId))
        {
            id = participantId;
        }

        public ConversationParticipantGraphType(ConversationParticipant conversationParticipant) : base(conversationParticipant)
        {
            id = conversationParticipant.Id;
        }

        public Id Id => Id.New<ConversationParticipant>(id);

        public Task<string> Sid => WithData(x => x.Sid);

        public async Task<ConversationGraphType> Conversation(IAppUserContext ctx)
        {
            var data = await Data;
            return new ConversationGraphType(ctx, data.ConversationId);
        }

        public async Task<UserGraphType?> User(IAppUserContext ctx)
        {
            var data = await Data;
            return data.User != null
                ? new UserGraphType(data.User)
                : data.UserId != null ? new UserGraphType(ctx, data.UserId) : null;
        }
    }
}
