using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
    public class AdminInviteEmail : EmailModel
    {
        public string FirstName { get; set; }
        public string InviteToken { get; set; }

        public override string Subject => "Vous avez été invité comme administrateur";

        public AdminInviteEmail(string to) : base(to)
        {
        }
    }
}