using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
    public class UserInviteEmail : EmailModel
    {
        public string FirstName { get; set; }
        public string InviteToken { get; set; }

        public override string Subject => "Vous avez été invité comme utilisateur";

        public UserInviteEmail(string to) : base(to)
        {
        }
    }
}
