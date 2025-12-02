using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
  public class WorkforceReviewNotificationEmail : EmailModel
  {
    public string AdUrl { get; set; }
    public string AdTitle { get; set; }
    public string CreatorUserName { get; set; }

    public override string Subject => "Nouvelle annonce Main d'oeuvres à valider - MutuAli";

    public WorkforceReviewNotificationEmail(string to) : base(to)
    {
    }
  }
}

