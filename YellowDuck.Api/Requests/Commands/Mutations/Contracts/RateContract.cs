using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Contracts
{
    public class RateContract : IRequestHandler<RateContract.Input, RateContract.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<RateContract> logger;
        private readonly UserManager<AppUser> userManager;
        private readonly ICurrentUserAccessor currentUserAccessor;

        public RateContract(AppDbContext db, ILogger<RateContract> logger, UserManager<AppUser> userManager, ICurrentUserAccessor currentUserAccessor)
        {
            this.db = db;
            this.logger = logger;
            this.userManager = userManager;
            this.currentUserAccessor = currentUserAccessor;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            var contractId = request.ContractId.LongIdentifierForType<Contract>();
            var contract = db.Contracts.Where(x => x.Id == contractId)
                .Include(x => x.Conversation)
                .Include(x => x.AdRating)
                .Include(x => x.UserRatings)
                .FirstOrDefault();

            if (contract == null)
            {
                throw new ContractNotFound();
            }

            var currentUserId = currentUserAccessor.GetCurrentUserId();

            if (contract.UserRatings.Where(x => x.RaterUserId == currentUserId).Any())
            {
                throw new ContractAlreadyRated();
            }

            if (contract.TenantId == currentUserId)
            {
                if(contract.AdRating != null)
                {
                    throw new ContractAlreadyRated();
                }

                request.AdRating.IfSet(x =>
                {
                    contract.AdRating = new AdRating
                    {
                        AdId = contract.Conversation.AdId,
                        RaterUserId = currentUserId,
                        ComplianceRating = x.Compliance,
                        CleanlinessRating = x.Cleanliness,
                        SecurityRating = x.Security,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                });
            }

            contract.UserRatings.Add(new UserRating
            {
                UserId = contract.OwnerId == currentUserId ? contract.TenantId : contract.OwnerId,
                RaterUserId = currentUserId,
                RespectRating = request.UserRating.Respect,
                CommunicationRating = request.UserRating.Communication,
                FiabilityRating = request.UserRating.Fiability,
                CreatedAtUtc = DateTime.UtcNow
            });

            await db.SaveChangesAsync(cancellationToken);

            if (contract.TenantId == currentUserId)
                logger.LogInformation($"Contract rated ({contract.Id}) by tenant {currentUserId}");
            else
                logger.LogInformation($"Contract rated ({contract.Id}) by owner {currentUserId}");

            return new Payload
            {
                Contract = new ContractGraphType(contract)
            };
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id ContractId { get; set; }
            public UserRatingInput UserRating { get; set; }
            public Maybe<AdRatingInput> AdRating { get; set; }
        }

        [InputType]
        public class AdRatingInput
        {
            public Rating Compliance { get; set; }
            public Rating Cleanliness { get; set; }
            public Rating Security { get; set; }
        }

        [InputType]
        public class UserRatingInput
        {
            public Rating Respect { get; set; }
            public Rating Communication { get; set; }
            public Rating Fiability { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public ContractGraphType Contract { get; set; }
        }

        public abstract class RateContractException : RequestValidationException { }

        public class ContractNotFound : RateContractException { }

        public class ContractAlreadyRated : RateContractException { }

    }
}
