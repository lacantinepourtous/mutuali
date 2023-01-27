using System.ComponentModel.DataAnnotations;
using YellowDuck.Api.DbModel.Entities.Contracts;

namespace YellowDuck.Api.DbModel.Entities.Payment
{
    public class CheckoutSession : IHaveLongIdentifier
    {
        public long Id { get; set; }

        [Required]
        public long ContractId { get; set; }
        public Contract Contract { get; set; }

        public string CheckoutSessionId { get; set; }
    }
}
