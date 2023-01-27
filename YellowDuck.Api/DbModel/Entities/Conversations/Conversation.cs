using System.Collections.Generic;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Contracts;

namespace YellowDuck.Api.DbModel.Entities.Conversations
{
    public class Conversation : IHaveLongIdentifier
    {
        public long Id { get; set; }

        public long AdId { get; set; }
        public Ad Ad { get; set; }

        public string Sid { get; set; }
        public IList<ConversationParticipant> Participants { get; set; }

        public long? ContractId { get; set; }
        public Contract Contract { get; set; }
    }
}
