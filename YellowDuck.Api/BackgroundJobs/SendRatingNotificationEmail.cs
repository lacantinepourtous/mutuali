using GraphQL.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Helpers;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.BackgroundJobs
{
  public class SendRatingNotificationEmail
  {
    private readonly AppDbContext db;
    private readonly IMailer mailer;
    private readonly ILogger<SendRatingNotificationEmail> logger;
    private readonly IConfiguration config;

    public SendRatingNotificationEmail(AppDbContext db, IMailer mailer, ILogger<SendRatingNotificationEmail> logger, IConfiguration config)
    {
      this.db = db;
      this.mailer = mailer;
      this.logger = logger;
      this.config = config;
    }

    public async Task Run(long? userRatingId, long? adRatingId, string userId, long? adId)
    {
      var kpisEmailRecipient = config["kpisEmailRecipient"];
      if (string.IsNullOrWhiteSpace(kpisEmailRecipient))
      {
        logger.LogWarning("kpisEmailRecipient is not configured, skipping rating notification email");
        return;
      }

      var email = new RatingNotificationEmail(kpisEmailRecipient)
      {
        UserRatingId = userRatingId,
        AdRatingId = adRatingId
      };

      // Informations pour le rating utilisateur
      if (userRatingId.HasValue && !string.IsNullOrEmpty(userId))
      {
        var ratedUserProfile = await db.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (ratedUserProfile != null)
        {
          email.UserProfileUrl = $"profil/{Id.New<UserProfile>(ratedUserProfile.Id)}";
          email.RatedUserName = ratedUserProfile.PublicName;
        }

        var userRating = await db.UserRatings
            .FirstOrDefaultAsync(x => x.Id == userRatingId.Value);

        if (userRating != null)
        {
          email.RaterUserName ??= await GetUserPublicName(userRating.RaterUserId);
        }
      }

      // Informations pour le rating annonce
      if (adRatingId.HasValue && adId.HasValue)
      {
        var ad = await db.Ads
            .Include(a => a.Translations)
            .FirstOrDefaultAsync(a => a.Id == adId.Value);

        if (ad != null)
        {
          email.AdUrl = UrlHelper.Ad(Id.New<Ad>(adId.Value));
          email.AdTitle = ad.Translations?.FirstOrDefault()?.Title ?? "Annonce sans titre";
        }

        var adRating = await db.AdRatings
            .FirstOrDefaultAsync(x => x.Id == adRatingId.Value);

        if (adRating != null)
        {
          email.RaterUserName ??= await GetUserPublicName(adRating.RaterUserId);
        }
      }

      try
      {
        await mailer.Send(email);
        logger.LogInformation($"Rating notification email sent to {kpisEmailRecipient}");
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Failed to send rating notification email");
      }
    }

    private async Task<string> GetUserPublicName(string userId)
    {
      var user = await db.Users
          .Include(u => u.Profile)
          .FirstOrDefaultAsync(u => u.Id == userId);

      return user?.Profile?.PublicName ?? string.Empty;
    }
  }
}

