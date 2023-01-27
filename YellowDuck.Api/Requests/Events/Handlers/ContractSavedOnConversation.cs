using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.Services.Twilio.Conversations;
using YellowDuck.Api.Services.Twilio.Conversations.Models;

namespace YellowDuck.Api.Requests.Events.Handlers
{
    public class ContractSavedOnConversation : INotificationHandler<ContractSaved>
    {
        private readonly IConversationsService conversationsService;

        public ContractSavedOnConversation(IConversationsService conversationsService)
        {
            this.conversationsService = conversationsService;
        }

        // TODO :: Translation
        public async Task Handle(ContractSaved notification, CancellationToken cancellationToken)
        {
            var body = "Le contrat a été modifié.";
            var attributes = new MessageAttributes
            {
                Action = MessageActions.ContractUpdated
            };
            await conversationsService.SendMessageAsSystem(notification.ConversationSid, body);
        }
    }
}
