using System.ComponentModel.DataAnnotations;
using YellowDuck.Api.DbModel.Entities.Contracts;

namespace YellowDuck.Api.DbModel.Entities.Payment
{
    public class Payout
    {
        public long Id { get; set; }

        [Required]
        public long ContractId { get; set; }
        public Contract Contract { get; set; }

        public string TransferId { get; set; }
    }
}
