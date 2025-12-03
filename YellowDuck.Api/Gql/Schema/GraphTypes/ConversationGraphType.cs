using System;
using GraphQL.Conventions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.Gql.Interfaces;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class ConversationGraphType : LazyGraphType<Conversation>
    {
        private readonly long id;

        public ConversationGraphType(IAppUserContext ctx, long conversationId) : base(() => ctx.LoadConversation(conversationId))
        {
            id = conversationId;
        }

        public ConversationGraphType(Conversation conversation) : base(conversation)
        {
            id = conversation.Id;
        }

        public Id Id => Id.New<Conversation>(id);

        public Task<string> Sid => WithData(x => x.Sid);

        public async Task<AdGraphType> Ad(IAppUserContext ctx)
        {
            var data = await Data;
            return new AdGraphType(ctx, data.AdId);
        }

        public async Task<IEnumerable<ConversationParticipantGraphType>> Participants(IAppUserContext ctx)
        {
            var participants = await ctx.LoadParticipants(id);
            return participants.Select(x => new ConversationParticipantGraphType(x)).ToList();
        }

        public async Task<ContractGraphType> Contract(IAppUserContext ctx)
        {
            var data = await Data;

            if (data.ContractId == null)
            {
                return null;
            }

            return data.Contract != null
                ? new ContractGraphType(data.Contract)
                : new ContractGraphType(ctx, data.ContractId.Value);
        }

        public async Task<IEnumerable<AdRatingGraphType>> AdRating(IAppUserContext ctx)
        {
            var adRatings = await ctx.LoadAdRatingByConversationId(id);
            return adRatings.Select(x => new AdRatingGraphType(x)).ToList();
        }

        public async Task<IEnumerable<UserRatingGraphType>> UserRatings(IAppUserContext ctx)
        {
            var userRatings = await ctx.LoadUserRatingByConversationId(id);
            return userRatings.Select(x => new UserRatingGraphType(x)).ToList();
        }

        public Task<DateTime?> RatingRequestSentAt => WithData(x => x.RatingRequestSentAt);
        
        public Task<string> RatingRequestJobId => WithData(x => x.RatingRequestJobId);
    }
}
