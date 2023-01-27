using System.ComponentModel.DataAnnotations;

namespace YellowDuck.Api.DbModel.Entities.Payment
{
    public class StripeAccount : IHaveLongIdentifier
    {
        public long Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string StripeAccountId { get; set; }
    }
}
