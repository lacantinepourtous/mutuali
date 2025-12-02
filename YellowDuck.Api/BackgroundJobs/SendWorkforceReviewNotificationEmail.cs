using GraphQL.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Helpers;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.BackgroundJobs
{
  public class SendWorkforceReviewNotificationEmail
  {
    private readonly AppDbContext db;
    private readonly IMailer mailer;
    private readonly ILogger<SendWorkforceReviewNotificationEmail> logger;
    private readonly IConfiguration config;

    public SendWorkforceReviewNotificationEmail(AppDbContext db, IMailer mailer, ILogger<SendWorkforceReviewNotificationEmail> logger, IConfiguration config)
    {
      this.db = db;
      this.mailer = mailer;
      this.logger = logger;
      this.config = config;
    }

    public async Task Run(long adId, string creatorUserId)
    {
      var adminEmailRecipient = config["adminEmailRecipient"];
      if (string.IsNullOrWhiteSpace(adminEmailRecipient))
      {
        logger.LogWarning("adminEmailRecipient is not configured, skipping workforce review notification email");
        return;
      }

      var email = new WorkforceReviewNotificationEmail(adminEmailRecipient)
      {
        AdUrl = UrlHelper.Ad(Id.New<Ad>(adId))
      };

      // Récupérer les traductions pour obtenir le titre
      var adWithTranslations = await db.Ads
          .Include(a => a.Translations)
          .FirstOrDefaultAsync(a => a.Id == adId);

      if (adWithTranslations != null)
      {
        email.AdTitle = adWithTranslations.Translations?.FirstOrDefault()?.Title ?? "Annonce sans titre";
      }

      // Obtenir le nom public du créateur
      var creatorWithProfile = await db.Users
          .Include(u => u.Profile)
          .FirstOrDefaultAsync(u => u.Id == creatorUserId);

      if (creatorWithProfile?.Profile != null &&
          !string.IsNullOrWhiteSpace(creatorWithProfile.Profile.FirstName) &&
          !string.IsNullOrWhiteSpace(creatorWithProfile.Profile.LastName))
      {
        email.CreatorUserName = creatorWithProfile.Profile.PublicName;
      }
      else if (creatorWithProfile != null)
      {
        email.CreatorUserName = creatorWithProfile.Email ?? "Utilisateur inconnu";
      }
      else
      {
        email.CreatorUserName = "Utilisateur inconnu";
      }

      try
      {
        await mailer.Send(email);
        logger.LogInformation($"Workforce review notification email sent to {adminEmailRecipient} for ad {adId}");
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Failed to send workforce review notification email");
      }
    }
  }
}

