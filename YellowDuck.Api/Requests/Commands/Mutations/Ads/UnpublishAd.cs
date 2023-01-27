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
    public class UnpublishAd : IRequestHandler<UnpublishAd.Input, UnpublishAd.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<UnpublishAd> logger;

        public UnpublishAd(AppDbContext db, ILogger<UnpublishAd> logger)
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

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Ad unpublished ({ad.Id})");

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

        public abstract class UnpublishAdException : RequestValidationException { }

        public class AdNotFoundException : UnpublishAdException { }
    }
}
