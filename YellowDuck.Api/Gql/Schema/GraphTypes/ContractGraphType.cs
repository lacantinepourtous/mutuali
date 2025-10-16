using GraphQL.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Services.Files;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class ContractGraphType : LazyGraphType<Contract>
    {
        private readonly long id;

        public ContractGraphType(IAppUserContext ctx, long contractId) : base(() => ctx.LoadContract(contractId))
        {
            id = contractId;
        }

        public ContractGraphType(Contract contract) : base(contract)
        {
            id = contract.Id;
        }

        public async Task<ConversationGraphType> Conversation(IAppUserContext ctx)
        {
            var data = await Data;

            return data.Conversation != null
                ? new ConversationGraphType(data.Conversation)
                : new ConversationGraphType(ctx, data.ConversationId);
        }

        public Id Id => Id.New<Contract>(id);

        public Task<DateTime> StartDate => WithData(x => x.StartDate);
        public Task<DateTime> EndDate => WithData(x => x.EndDate);
        public Task<string> DatePrecision => WithData(x => x.DatePrecision);

        public Task<double> Price => WithData(x => x.Price);
        public Task<string> Terms => WithData(x => x.Terms);

        public async Task<IEnumerable<string>> Files(
            IAppUserContext ctx,
            [Inject] FileUrlProvider urlProvider)
        {
            var fileItems = await ctx.LoadContractFileItems(id);
            return fileItems.Select(x => urlProvider.GetFileUrl(FileContainers.Files, x.FileId, new TimeSpan(24, 0, 0))).ToList();
        }

        public async Task<UserGraphType> Owner(IAppUserContext ctx)
        {
            var data = await Data;

            return data.Owner != null
                ? new UserGraphType(data.Owner)
                : new UserGraphType(ctx, data.OwnerId);
        }

        public async Task<UserGraphType> Tenant(IAppUserContext ctx)
        {
            var data = await Data;

            return data.Tenant != null
                ? new UserGraphType(data.Tenant)
                : new UserGraphType(ctx, data.TenantId);
        }

        public Task<ContractStatus> Status => WithData(x => x.Status);

        public async Task<CheckoutSessionGraphType> CheckoutSession(IAppUserContext ctx)
        {
            var checkoutSession = await ctx.LoadCheckoutSessionByContractId(id);
            return (checkoutSession != null) ? new CheckoutSessionGraphType(checkoutSession) : null;
        }
    }
}
