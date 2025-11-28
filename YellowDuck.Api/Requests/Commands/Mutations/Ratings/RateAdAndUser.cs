using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Helpers;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Services.Mailer;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ratings
{
  public class RateAdAndUser : IRequestHandler<RateAdAndUser.Input, RateAdAndUser.Payload>
  {
    private readonly AppDbContext db;
    private readonly ILogger<RateAdAndUser> logger;
    private readonly UserManager<AppUser> userManager;
    private readonly ICurrentUserAccessor currentUserAccessor;
    private readonly IMailer mailer;
    private readonly IConfiguration config;

    public RateAdAndUser(AppDbContext db, ILogger<RateAdAndUser> logger, UserManager<AppUser> userManager, ICurrentUserAccessor currentUserAccessor, IMailer mailer, IConfiguration config)
    {
      this.db = db;
      this.logger = logger;
      this.userManager = userManager;
      this.currentUserAccessor = currentUserAccessor;
      this.mailer = mailer;
      this.config = config;
    }

    public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
    {
      var currentUserId = currentUserAccessor.GetCurrentUserId();
      var userId = request.UserId.IdentifierForType<AppUser>();
      var adId = request.AdId.LongIdentifierForType<Ad>();
      var conversationId = request.ConversationId.LongIdentifierForType<Conversation>();

      // Vérifier si l'utilisateur a déjà noté cet utilisateur
      var existingUserRating = await db.UserRatings
          .FirstOrDefaultAsync(x => x.UserId == userId && x.RaterUserId == currentUserId, cancellationToken);

      if (existingUserRating != null)
      {
        throw new UserAlreadyRated();
      }

      // Créer la note pour l'utilisateur
      var userRating = new UserRating
      {
        UserId = userId,
        RaterUserId = currentUserId,
        ConversationId = conversationId,
        RespectRating = request.UserRating.Respect,
        CommunicationRating = request.UserRating.Communication,
        OverallRating = request.UserRating.Overall,
        Comment = request.UserRating.Comment,
        CreatedAtUtc = DateTime.UtcNow,
        LastUpdatedAtUtc = DateTime.UtcNow
      };

      db.UserRatings.Add(userRating);

      // Créer la note pour l'annonce seulement si AdRating est fourni
      AdRating adRating = null;
      if (request.AdRating.IsSet())
      {
        // Vérifier si l'utilisateur a déjà noté cette annonce
        var existingAdRating = await db.AdRatings
            .FirstOrDefaultAsync(x => x.AdId == adId && x.RaterUserId == currentUserId, cancellationToken);

        if (existingAdRating != null)
        {
          throw new AdAlreadyRated();
        }

        adRating = new AdRating
        {
          AdId = adId,
          RaterUserId = currentUserId,
          ConversationId = conversationId,
          ComplianceRating = request.AdRating.Value.Compliance,
          QualityRating = request.AdRating.Value.Quality,
          OverallRating = request.AdRating.Value.Overall,
          Comment = request.AdRating.Value.Comment,
          CreatedAtUtc = DateTime.UtcNow,
          LastUpdatedAtUtc = DateTime.UtcNow
        };

        db.AdRatings.Add(adRating);
      }

      await db.SaveChangesAsync(cancellationToken);

      logger.LogInformation($"User {userId} rated by user {currentUserId}" + (adRating != null ? $", Ad {adId} also rated" : ""));

      // Envoyer l'email de notification
      await SendRatingNotificationEmail(userRating, adRating, userId, adId, cancellationToken);

      return new Payload
      {
        AdRating = adRating != null ? new AdRatingGraphType(adRating) : null,
        UserRating = new UserRatingGraphType(userRating)
      };
    }

    [MutationInput]
    public class Input : IRequest<Payload>
    {
      public Id UserId { get; set; }
      public Id AdId { get; set; }
      public Id ConversationId { get; set; }
      public UserRatingInput UserRating { get; set; }
      public Maybe<AdRatingInput> AdRating { get; set; }
    }

    [InputType]
    public class AdRatingInput
    {
      public Rating Compliance { get; set; }
      public Rating Quality { get; set; }
      public Rating Overall { get; set; }
      public string Comment { get; set; }
    }

    [InputType]
    public class UserRatingInput
    {
      public Rating Respect { get; set; }
      public Rating Communication { get; set; }
      public Rating Overall { get; set; }
      public string Comment { get; set; }
    }

    [MutationPayload]
    public class Payload
    {
      public AdRatingGraphType AdRating { get; set; }
      public UserRatingGraphType UserRating { get; set; }
    }

    private async Task SendRatingNotificationEmail(UserRating userRating, AdRating adRating, string userId, long adId, CancellationToken cancellationToken)
    {
      var adminEmailRecipient = config["adminEmailRecipient"];
      if (string.IsNullOrWhiteSpace(adminEmailRecipient))
      {
        logger.LogWarning("adminEmailRecipient is not configured, skipping rating notification email");
        return;
      }

      var email = new RatingNotificationEmail(adminEmailRecipient)
      {
        UserRatingId = userRating?.Id,
        AdRatingId = adRating?.Id
      };

      // Informations pour le rating utilisateur
      if (userRating != null)
      {
        var ratedUserProfile = await db.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (ratedUserProfile != null)
        {
          email.UserProfileUrl = $"profil/{Id.New<UserProfile>(ratedUserProfile.Id)}";
          email.RatedUserName = ratedUserProfile.PublicName;
        }

        email.RaterUserName ??= await GetUserPublicName(userRating.RaterUserId, cancellationToken);
      }

      // Informations pour le rating annonce
      if (adRating != null)
      {
        var ad = await db.Ads
            .Include(a => a.Translations)
            .FirstOrDefaultAsync(a => a.Id == adId, cancellationToken);

        if (ad != null)
        {
          email.AdUrl = UrlHelper.Ad(Id.New<Ad>(adId));
          email.AdTitle = ad.Translations?.FirstOrDefault()?.Title ?? "Annonce sans titre";
        }

        email.RaterUserName ??= await GetUserPublicName(adRating.RaterUserId, cancellationToken);
      }

      try
      {
        await mailer.Send(email);
        logger.LogInformation($"Rating notification email sent to {adminEmailRecipient}");
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Failed to send rating notification email");
      }
    }

    private async Task<string> GetUserPublicName(string userId, CancellationToken cancellationToken)
    {
      var user = await db.Users
          .Include(u => u.Profile)
          .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

      return user?.Profile?.PublicName ?? string.Empty;
    }

    public abstract class RateAdAndUserException : Exception { }
    public class UserAlreadyRated : RateAdAndUserException { }
    public class AdAlreadyRated : RateAdAndUserException { }
  }
}