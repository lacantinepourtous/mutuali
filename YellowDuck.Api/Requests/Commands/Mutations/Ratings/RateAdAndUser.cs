using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ratings
{
  public class RateAdAndUser : IRequestHandler<RateAdAndUser.Input, RateAdAndUser.Payload>
  {
    private readonly AppDbContext db;
    private readonly ILogger<RateAdAndUser> logger;
    private readonly UserManager<AppUser> userManager;
    private readonly ICurrentUserAccessor currentUserAccessor;

    public RateAdAndUser(AppDbContext db, ILogger<RateAdAndUser> logger, UserManager<AppUser> userManager, ICurrentUserAccessor currentUserAccessor)
    {
      this.db = db;
      this.logger = logger;
      this.userManager = userManager;
      this.currentUserAccessor = currentUserAccessor;
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
        CreatedAtUtc = DateTime.UtcNow,
        LastUpdatedAtUtc = DateTime.UtcNow
      };

      db.UserRatings.Add(userRating);

      // Créer la note pour l'annonce seulement si AdRating est fourni
      AdRating adRating = null;
      if (request.AdRating != null)
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
          ComplianceRating = request.AdRating.Compliance,
          QualityRating = request.AdRating.Quality,
          OverallRating = request.AdRating.Overall,
          CreatedAtUtc = DateTime.UtcNow,
          LastUpdatedAtUtc = DateTime.UtcNow
        };

        db.AdRatings.Add(adRating);
      }

      await db.SaveChangesAsync(cancellationToken);

      logger.LogInformation($"User {userId} rated by user {currentUserId}" + (adRating != null ? $", Ad {adId} also rated" : ""));

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
      public AdRatingInput AdRating { get; set; }
    }

    [InputType]
    public class AdRatingInput
    {
      public Rating Compliance { get; set; }
      public Rating Quality { get; set; }
      public Rating Overall { get; set; }
    }

    [InputType]
    public class UserRatingInput
    {
      public Rating Respect { get; set; }
      public Rating Communication { get; set; }
      public Rating Overall { get; set; }
    }

    [MutationPayload]
    public class Payload
    {
      public AdRatingGraphType AdRating { get; set; }
      public UserRatingGraphType UserRating { get; set; }
    }

    public abstract class RateAdAndUserException : Exception { }
    public class UserAlreadyRated : RateAdAndUserException { }
    public class AdAlreadyRated : RateAdAndUserException { }
  }
}