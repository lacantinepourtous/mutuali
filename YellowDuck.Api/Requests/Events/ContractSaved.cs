using MediatR;

namespace YellowDuck.Api.Requests.Events
{
    public class ContractSaved : INotification
    {
        public string ConversationSid { get; set; }

        public string Owner { get; set; }
        public string Tenant { get; set; }
    }
}
