using MediatR;

namespace YellowDuck.Api.Requests.Events
{
    public class MessageAdded : INotification
    {
        public string ConversationSid { get; set; }

        public string Body { get; set; }

        public string AuthorId { get; set; }
    }
}
