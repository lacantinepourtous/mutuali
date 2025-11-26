using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Twilio.Rest.Conversations.V1.Service.Conversation;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.Services.Twilio;
using YellowDuck.Api.Services.Twilio.Conversations;
using YellowDuck.Api.Services.Twilio.Conversations.Models;

namespace YellowDuck.Api.BackgroundJobs
{
    /// <summary>
    /// Sends an automatic system message in a conversation 48 hours after
    /// both users have sent at least one message, asking them to rate each other.
    /// This job is scheduled individually per conversation when a message is sent.
    /// </summary>
    public class SendConversationRatingRequestMessage
    {
        private readonly AppDbContext db;
        private readonly ITwilioService twilioService;
        private readonly IConversationsService conversationsService;
        private readonly ILogger<SendConversationRatingRequestMessage> logger;

        public SendConversationRatingRequestMessage(
            AppDbContext db,
            ITwilioService twilioService,
            IConversationsService conversationsService,
            ILogger<SendConversationRatingRequestMessage> logger)
        {
            this.db = db;
            this.twilioService = twilioService;
            this.conversationsService = conversationsService;
            this.logger = logger;
        }

        public async Task Run(long conversationId)
        {
            try
            {
                var conversation = await db.Conversations
                    .Where(c => c.Id == conversationId && c.Sid != null)
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync();

                if (conversation == null)
                {
                    logger.LogWarning($"Conversation {conversationId} not found or has no Twilio SID.");
                    return;
                }

                var userParticipants = conversation.Participants
                    .Where(p => p.UserId != null)
                    .ToList();

                // We only care about conversations between at least two users
                if (userParticipants.Count < 2)
                {
                    logger.LogInformation($"Conversation {conversationId} has less than 2 user participants, skipping rating request.");
                    return;
                }

                var chatServiceSid = twilioService.GetChatServiceSid();

                // Read Twilio messages for this conversation
                var messages = MessageResource.Read(
                    pathChatServiceSid: chatServiceSid,
                    pathConversationSid: conversation.Sid,
                    limit: 100 // reasonable cap; conversations are usually short
                ).ToList();

                if (!messages.Any())
                {
                    logger.LogInformation($"No messages found for conversation {conversationId}, skipping rating request.");
                    return;
                }

                // Skip if we already sent a rating request system message
                var alreadyHasRatingRequest = messages.Any(m =>
                    m.Author == TwilioEnums.ParticipantSystem &&
                    !string.IsNullOrEmpty(m.Attributes) &&
                    m.Attributes.Contains($"\"{nameof(MessageAttributes.Action)}\":\"{MessageActions.RatingRequest}\""));

                if (alreadyHasRatingRequest)
                {
                    logger.LogInformation($"Conversation {conversationId} already has a rating request message, skipping.");
                    return;
                }

                // Send one system message visible to both users
                // This message will be overrided by a translation key on the front-end side.
                var body = "La conversation a eu lieu il y a quelques jours. Il est maintenant temps d'évaluer votre expérience.";
                var attributes = new MessageAttributes
                {
                    Action = MessageActions.RatingRequest
                };

                await conversationsService.SendMessageAsSystem(conversation.Sid, body, attributes);

                logger.LogInformation($"Sent conversation rating request for conversation {conversation.Id} (Twilio SID {conversation.Sid}).");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to process rating request for conversation {conversationId}.");
            }
        }
    }
}


