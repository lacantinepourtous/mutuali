using MediatR;

namespace YellowDuck.Api.Requests.Events
{
    public class ContractCreated : INotification
    {
        public string ConversationSid { get; set; }

        public string Owner { get; set; }
        public string Tenant { get; set; }
    }
}
