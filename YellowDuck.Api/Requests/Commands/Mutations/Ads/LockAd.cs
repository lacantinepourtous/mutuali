using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ads
{
  public class LockAd : IRequestHandler<LockAd.Input, LockAd.Payload>
  {
    private readonly AppDbContext db;
    private readonly ILogger<LockAd> logger;

    public LockAd(AppDbContext db, ILogger<LockAd> logger)
    {
      this.db = db;
      this.logger = logger;
    }

    public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
    {
      var adId = request.AdId.LongIdentifierForType<Ad>();

      var ad = await db.Ads.FirstOrDefaultAsync(x => x.Id == adId, cancellationToken);

      if (ad == null)
      {
        throw new AdNotFoundException();
      }

      ad.IsPublish = false;
      ad.Locked = true;

      await db.SaveChangesAsync(cancellationToken);

      logger.LogInformation($"Ad locked ({ad.Id})");

      return new Payload
      {
        Ad = new AdGraphType(ad)
      };
    }

    [MutationInput]
    public class Input : IRequest<Payload>, IHaveAdId
    {
      public Id AdId { get; set; }
    }

    [MutationPayload]
    public class Payload
    {
      public AdGraphType Ad { get; set; }
    }

    public abstract class LockAdException : RequestValidationException { }

    public class AdNotFoundException : LockAdException { }
  }
}