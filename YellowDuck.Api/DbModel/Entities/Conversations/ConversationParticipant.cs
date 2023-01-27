namespace YellowDuck.Api.DbModel.Entities.Conversations
{
    public class ConversationParticipant : IHaveLongIdentifier
    {
        public long Id { get; set; }

        public string Sid { get; set; }
        public long ConversationId { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
