using GraphQL.Conventions;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
    public class AlertEmail : EmailModel
    {
        public override string Subject => $"Alerte Mutuali: {AdsCount} offre(s) pourrai(en)t vous intéresser.";
        public Id AlertIdentifier { get; set; }
        public AdCategory Category { get; set; }
        public int AdsCount { get; set; }
        public bool CanManageAlert { get; set; }
        public string CtaUrlFr { get; set; }
        public string CtaUrlEn { get; set; }

        public AlertEmail(string to) : base(to) { }
    }
}
