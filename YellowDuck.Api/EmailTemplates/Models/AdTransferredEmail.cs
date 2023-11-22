using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
    public class AdTransferredEmail : EmailModel
    {
        public override string Subject => "Une annonce vous a été transférée / An ad has been transferred to you";

        public string Title { get; set; }
        public string AdId { get; set; }

        public AdTransferredEmail(string to, string title, string adId) : base(to)
        {
            Title = title;
            AdId = adId;
        }
    }
}
