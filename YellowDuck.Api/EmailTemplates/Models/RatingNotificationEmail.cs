using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
  public class RatingNotificationEmail : EmailModel
  {
    public long? UserRatingId { get; set; }
    public long? AdRatingId { get; set; }
    public string UserProfileUrl { get; set; }
    public string AdUrl { get; set; }
    public string RaterUserName { get; set; }
    public string RatedUserName { get; set; }
    public string AdTitle { get; set; }

    public override string Subject => "Nouvelle évaluation - MutuAli";

    public RatingNotificationEmail(string to) : base(to)
    {
    }
  }
}

