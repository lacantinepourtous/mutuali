using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Enums;
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
    public class CreateContract : IRequestHandler<CreateContract.Input, CreateContract.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<CreateContract> logger;
        private readonly IFileManager fileManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly IMediator mediator;

        public CreateContract(AppDbContext db, ILogger<CreateContract> logger, IFileManager fileManager, UserManager<AppUser> userManager, ICurrentUserAccessor currentUserAccessor, IMediator mediator)
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

            var conversationId = request.ConversationId.LongIdentifierForType<Conversation>();
            var conversation = db.Conversations.Where(x => x.Id == conversationId)
                .Include(x => x.Ad)
                .Include(x => x.Participants).ThenInclude(x => x.User).ThenInclude(x => x.Profile)
                .FirstOrDefault();
            
            if (conversation == null)
            {
                throw new ConversationNotFound();
            }

            if (conversation.ContractId != null)
            {
                throw new ConversationAlreadyHaveContract();
            }

            var owner = conversation.Participants.Where(x => x.UserId != null && x.UserId == conversation.Ad.UserId).First().User;
            if (owner.Id != currentUserAccessor.GetCurrentUserId()) throw new UserNotOwner();
            var tenant = conversation.Participants.Where(x => x.UserId != null && x.UserId != owner.Id).First().User;

            var contract = new Contract()
            {
                OwnerId = owner.Id,
                TenantId = tenant.Id,
                ConversationId = conversation.Id,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Price = request.Price,
                Status = ContractStatus.Submitted,
                Terms = request.Terms,
                DatePrecision = request.DatePrecision
            };

            request.FileItems.IfSet(x =>
            {
                contract.Files = new List<ContractFileItem>();
                x.Value.ForEach(y => contract.Files.Add(new ContractFileItem() { FileId = y }));
            });

            conversation.Contract = contract;
            
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Contract for conversation {conversation.Id}, created by user {owner.Id}");

            await mediator.Publish(new ContractCreated
            {
                ConversationSid = contract.Conversation.Sid,
                Owner = owner.Profile.PublicName,
                Tenant = tenant.Profile.PublicName
            });

            return new Payload
            {
                Contract = new ContractGraphType(contract)
            };
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
            public Id ConversationId { get; set; }
            public Maybe<NonNull<List<string>>> FileItems { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string DatePrecision { get; set; }

            public double Price { get; set; }
            public string Terms { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public ContractGraphType Contract { get; set; }
        }

        public abstract class CreateContractException : RequestValidationException { }

        public class ConversationNotFound : CreateContractException { }

        public class ConversationAlreadyHaveContract : CreateContractException { }

        public class UserNotOwner : CreateContractException { }


        public class FileNotFoundException : CreateContractException
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
