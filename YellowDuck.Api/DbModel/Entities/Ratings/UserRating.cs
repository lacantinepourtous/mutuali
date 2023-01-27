using System;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Ratings
{
    public class UserRating : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long ContractId { get; set; }
        public Contract Contract { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RaterUserId { get; set; }
        public AppUser RaterUser { get; set; }
        public Rating RespectRating { get; set; }
        public Rating CommunicationRating { get; set; }
        public Rating FiabilityRating { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
