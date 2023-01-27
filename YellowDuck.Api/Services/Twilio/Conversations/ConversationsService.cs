using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Twilio.Rest.Conversations.V1.Service;
using Twilio.Rest.Conversations.V1.Service.Conversation;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.Services.Twilio.Conversations.Models;

namespace YellowDuck.Api.Services.Twilio.Conversations
{
    public class ConversationsService : IConversationsService
    {
        private readonly ITwilioService TwilioService;
        private readonly IConfiguration Configuration;

        public ConversationsService(ITwilioService twilioService, IConfiguration configuration)
        {
            TwilioService = twilioService;
            Configuration = configuration;
        }

        public async Task<Conversation> Create(IEnumerable<AppUser> users)
        {
            var twilioConversation = await ConversationResource.CreateAsync(
                pathChatServiceSid: TwilioService.GetChatServiceSid()
            );

            var configurationFilters = new List<string> {
                "onMessageAdded"
            };

             await WebhookResource.CreateAsync(
                target: WebhookResource.TargetEnum.Webhook,
                pathChatServiceSid: TwilioService.GetChatServiceSid(),
                pathConversationSid: twilioConversation.Sid,
                configurationMethod: WebhookResource.MethodEnum.Post,
                configurationUrl: Configuration["Twilio:webHookUrl"],
                configurationFilters: configurationFilters
            );

            var conversation = new Conversation()
            {
                Sid = twilioConversation.Sid,
                Participants = new List<ConversationParticipant>()
            };

            foreach (var user in users)
            {
                var twilioParticipant = await ParticipantResource.CreateAsync(
                    pathChatServiceSid: TwilioService.GetChatServiceSid(),
                    pathConversationSid: twilioConversation.Sid,
                    identity: user.Id
                );

                conversation.Participants.Add(new ConversationParticipant()
                {
                    Sid = twilioParticipant.Sid,
                    UserId = user.Id
                });
            }

            var twilioParticipantSystem = await ParticipantResource.CreateAsync(pathChatServiceSid: TwilioService.GetChatServiceSid(),
                    pathConversationSid: twilioConversation.Sid,
                    identity: TwilioEnums.ParticipantSystem);
            
            conversation.Participants.Add(new ConversationParticipant()
            {
                Sid = twilioParticipantSystem.Sid
            });

            return conversation;
        }

        public async Task SendMessageAsSystem(string conversationSid, string body, MessageAttributes attributes = null)
        {
            var serializedAttributes = (attributes != null) ? JsonSerializer.Serialize(attributes) : null;
            await MessageResource.CreateAsync(
                pathChatServiceSid: TwilioService.GetChatServiceSid(),
                pathConversationSid: conversationSid,
                author: TwilioEnums.ParticipantSystem,
                attributes: serializedAttributes,
                body: body
            );
        }
    }
}
