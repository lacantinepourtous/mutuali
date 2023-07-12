using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ads
{
    public class TransferAd : IRequestHandler<TransferAd.Input, TransferAd.Payload>
    {
        private readonly AppDbContext db;
        private readonly UserManager<AppUser> userManager;
        private readonly IMailer mailer;
        private readonly ILogger<PublishAd> logger;

        public TransferAd(AppDbContext db, UserManager<AppUser> userManager, IMailer mailer, ILogger<PublishAd> logger)
        {
            this.db = db;
            this.userManager = userManager;
            this.mailer = mailer;
            this.logger = logger;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var adId = request.AdId.LongIdentifierForType<Ad>();

            var ad = await db.Ads.Include(x => x.User).Include(x => x.Translations).FirstOrDefaultAsync(x => x.Id == adId, cancellationToken);

            if (ad == null)
            {
                throw new AdNotFoundException();
            }

            if (ad.IsAdminOnly == false)
            {
                throw new AdNotAdminOnlyException();
            }

            var newOwner = db.Users.Where(x => x.Email == request.UserEmail).FirstOrDefault();

            if (newOwner == null)
            {
                throw new UserNotFoundException();
            }

            ad.IsAdminOnly = false;
            ad.IsPublish = false;
            ad.UserId = newOwner.Id;

            await db.SaveChangesAsync(cancellationToken);

            var claims = await userManager.GetClaimsAsync(ad.User);
            var adOwnerClaim = claims.Where(x => x.Type == AppClaimTypes.AdOwner && x.Value == Id.New<Ad>(ad.Id.ToString()).ToString()).FirstOrDefault();
            if(adOwnerClaim != null) await userManager.RemoveClaimAsync(ad.User, adOwnerClaim);

            await userManager.AddClaimAsync(newOwner, new Claim(AppClaimTypes.AdOwner, Id.New<Ad>(ad.Id.ToString()).ToString()));

            await mailer.Send(new AdTransferredEmail(newOwner.Email, ad.Translations.First().Title, Id.New<Ad>(ad.Id.ToString()).ToString()));

            logger.LogInformation($"Ad transfered to {newOwner.Id} ({ad.Id})");

            return new Payload
            {
                Ad = new AdGraphType(ad)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>, IHaveAdId
        {
            public Id AdId { get; set; }
            public string UserEmail { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public AdGraphType Ad { get; set; }
        }

        public abstract class TransferAdException : RequestValidationException { }

        public class AdNotFoundException : TransferAdException { }
        public class AdNotAdminOnlyException : TransferAdException { }
        public class UserNotFoundException : TransferAdException { }
    }
}
