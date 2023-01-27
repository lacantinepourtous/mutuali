using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
    public class ConfirmEmailEmail : EmailModel
    {
        public override string Subject => "Activation requise / Activation required";

        public string Token { get; set; }

        public string ReturnPath { get; set; }

        public ConfirmEmailEmail(string to, string token, string returnPath = "") : base(to)
        {
            Token = token;
            ReturnPath = returnPath;
        }
    }
}
