using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.Services.Twilio.Conversations;
using YellowDuck.Api.Services.Twilio.Conversations.Models;

namespace YellowDuck.Api.Requests.Events.Handlers
{
    public class ContractCreatedOnConversation : INotificationHandler<ContractCreated>
    {
        private readonly IConversationsService conversationsService;

        public ContractCreatedOnConversation(IConversationsService conversationsService)
        {
            this.conversationsService = conversationsService;
        }

        // TODO :: Translation
        public async Task Handle(ContractCreated notification, CancellationToken cancellationToken)
        {
            var body = $"Votre contrat est maintenant disponible. Il est important que {notification.Tenant} s'assure de le réviser attentivement et de demander des ajustements au besoin avant de l'accepter et de procéder au paiement.";
            var attributes = new MessageAttributes
            {
                Action = MessageActions.ContractCreated
            };
            await conversationsService.SendMessageAsSystem(notification.ConversationSid, body, attributes);
            body = $"Si des modifications doivent être apportées, il est possible pour {notification.Owner} de les faire tant que le paiement n'a pas été effectué. Après ce moment, le contrat sera verrouillé.";
            await conversationsService.SendMessageAsSystem(notification.ConversationSid, body);
        }
    }
}
