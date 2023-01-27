using System.Collections.Generic;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
    public class DailyConversationNotificationSummaryEmail : EmailModel
    {
        public override string Subject => $"Nouveaux messages reçus sur MutuAli / New messages received on MutuAli";
        public string ConversationUrl = "/conversations";
        public string AdUrl = "/annonces";

        public IList<Conversation> Conversations { get; set; }

        public DailyConversationNotificationSummaryEmail(string to) : base(to) { }

        public string GetConversationUrl(Conversation conversation, string lang = null)
        {
            var queryString = (lang != null) ? $"?lang={lang}" : "";
            return $"{BaseUrl}{ConversationUrl}/{conversation.Id}{queryString}";
        }

        public string GetAdUrl(Ad ad) => $"{BaseUrl}{AdUrl}/{ad.Id}";
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public class Conversation
        {
            public string Id { get; set; }
            public List<string> Bodies { get; set; }
            public string UserFrom { get; set; }
            public Ad Ad { get; set; }
        }

        public class Ad
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }
    }
}
