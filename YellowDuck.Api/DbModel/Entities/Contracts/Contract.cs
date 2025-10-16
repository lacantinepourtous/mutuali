using System;
using System.Collections.Generic;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Contracts
{
    public class Contract : IHaveLongIdentifier
    {
        public long Id { get; set; }

        public string OwnerId { get; set; }
        public AppUser Owner { get; set; }
        public string TenantId { get; set; }
        public AppUser Tenant { get; set; }

        public long ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DatePrecision { get; set; }

        public double Price { get; set; }
        public string Terms { get; set; }

        public IList<ContractFileItem> Files { get; set; }
        public ContractStatus Status { get; set; }

        public CheckoutSession CheckoutSession { get; set; }
        public Payout Payout { get; set; }
    }
}
