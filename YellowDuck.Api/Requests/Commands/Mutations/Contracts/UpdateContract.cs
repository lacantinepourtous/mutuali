using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Helpers;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Requests.Events;
using YellowDuck.Api.Services.Files;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Contracts
{
    public class UpdateContract : IRequestHandler<UpdateContract.Input, UpdateContract.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<UpdateContract> logger;
        private readonly IFileManager fileManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly IMediator mediator;

        public UpdateContract(AppDbContext db, ILogger<UpdateContract> logger, IFileManager fileManager, UserManager<AppUser> userManager, ICurrentUserAccessor currentUserAccessor, IMediator mediator)
        {
            this.db = db;
            this.logger = logger;
            this.fileManager = fileManager;
            this.userManager = userManager;
            this.currentUserAccessor = currentUserAccessor;
            this.mediator = mediator;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);

            var contractId = request.ContractId.LongIdentifierForType<Contract>();
            var contract = db.Contracts.Where(x => x.Id == contractId)
                .Include(x => x.Conversation)
                .Include(x => x.Files)
                .Include(x => x.Owner).ThenInclude(x => x.Profile)
                .Include(x => x.Tenant).ThenInclude(x => x.Profile)
                .FirstOrDefault();

            if (contract == null)
            {
                throw new ContractNotFound();
            }

            if (contract.Owner.Id != currentUserAccessor.GetCurrentUserId()) throw new UserNotOwner();

            request.StartDate.IfSet(v => contract.StartDate = v);
            request.EndDate.IfSet(v => contract.EndDate = v);
            request.DatePrecision.IfSet(v => contract.DatePrecision = v);

            request.Price.IfSet(v => contract.Price = v);
            request.Terms.IfSet(v => contract.Terms = v);

            request.FileItems.IfSet(v => UpdateFileItems(contract, v.Value));

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Contract updated ({contract.Id})");

            await mediator.Publish(new ContractSaved
            {
                ConversationSid = contract.Conversation.Sid,
                Owner = contract.Owner.Profile.PublicName,
                Tenant = contract.Tenant.Profile.PublicName
            });

            return new Payload
            {
                Contract = new ContractGraphType(contract)
            };
        }

        private void UpdateFileItems(Contract contract, List<string> fileItems)
        {
            contract.Files.ForEach(y => db.ContractFiles.Remove(y));
            contract.Files = new List<ContractFileItem>();
            fileItems.ForEach(y => contract.Files.Add(new ContractFileItem() { FileId = y }));
        }

        private async Task ValidateRequest(Input request)
        {
            try
            {
                if (request.FileItems.IsSet())
                {
                    var pictures = request.FileItems.Value;
                    for (var i = 0; i < pictures.Value.Count; i++)
                    {
                        await MutationHelper.CheckFileMaybe(pictures.Value[i], fileManager);
                    }
                }
            }
            catch
            {
                throw new FileNotFoundException("FileItems");
            }
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public Id ContractId { get; set; }
            public Maybe<NonNull<List<string>>> FileItems { get; set; }

            public Maybe<DateTime> StartDate { get; set; }
            public Maybe<DateTime> EndDate { get; set; }
            public Maybe<string> DatePrecision { get; set; }

            public Maybe<double> Price { get; set; }
            public Maybe<string> Terms { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public ContractGraphType Contract { get; set; }
        }

        public abstract class UpdateContractException : RequestValidationException { }

        public class ContractNotFound : UpdateContractException { }

        public class UserNotOwner : UpdateContractException { }

        public class FileNotFoundException : UpdateContractException
        {
            private readonly string propName;

            public FileNotFoundException(string propName)
            {
                this.propName = propName;
            }

            public override IDictionary Data => new Dictionary<string, string> {
                {"Property", propName}
            };
        }
    }
}
