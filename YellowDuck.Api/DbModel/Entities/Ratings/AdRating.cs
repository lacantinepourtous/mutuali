using System;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Ratings
{
    public class AdRating : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long ContractId { get; set; }
        public Contract Contract { get; set; }
        public long AdId { get; set; }
        public Ad Ad { get; set; }
        public string RaterUserId { get; set; }
        public AppUser RaterUser { get; set; }
        public Rating ComplianceRating { get; set; }
        public Rating CleanlinessRating { get; set; }
        public Rating SecurityRating { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
