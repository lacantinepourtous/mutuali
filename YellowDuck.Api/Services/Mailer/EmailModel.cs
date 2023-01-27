using System.Text.RegularExpressions;

namespace YellowDuck.Api.Services.Mailer
{
    public abstract class EmailModel
    {
        public string BaseUrl { get; set; }
        public string To { get; set; }
        public virtual string TemplateName => Regex.Replace(GetType().Name, "Email$", "");
        public abstract string Subject { get; }

        protected EmailModel(string to)
        {
            To = to;
        }
    }
}