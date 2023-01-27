using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
    public class ContactEmail : EmailModel
    {
        public string FullName { get; set; }
        public string OrganizationName { get; set; }
        public string EmailOrPhone { get; set; }
        public string Origin { get; set; }

        public override string Subject => $"Mutuali - Nouveau message - {Origin}";

        public ContactEmail(string to) : base(to)
        {
        }
    }
}