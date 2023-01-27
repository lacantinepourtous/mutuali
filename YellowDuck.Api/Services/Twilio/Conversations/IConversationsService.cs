using System.Collections.Generic;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.Services.Twilio.Conversations.Models;

namespace YellowDuck.Api.Services.Twilio.Conversations
{
    public interface IConversationsService
    {
        Task<Conversation> Create(IEnumerable<AppUser> users);
        Task SendMessageAsSystem(string conversationSid, string body, MessageAttributes attributes = null);
    }
}
