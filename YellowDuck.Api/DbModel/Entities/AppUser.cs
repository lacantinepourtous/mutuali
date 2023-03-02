using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.DbModel.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using YellowDuck.Api.DbModel.Entities.Ratings;
using System.Collections.Generic;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Contracts;

namespace YellowDuck.Api.DbModel.Entities
{
    public class AppUser : IdentityUser<string>, IHaveStringIdentifier
    {
        public AppUser() { }

        public AppUser(string userName)
        {
            UserName = userName;
        }

        public override string Email
        {
            get => UserName;
            set => base.Email = base.UserName = value;
        }

        public override string UserName
        {
            get => base.UserName;
            set => base.Email = base.UserName = value;
        }

        public UserType Type { get; set; }

        public UserProfile Profile { get; set; }
        public IList<Ad> Ads { get; set; }
        public IList<Contract> TenantContracts { get; set; }
        public IList<Contract> OwnerContracts { get; set; }
        public IList<UserRating> UserRatings { get; set; }
        public StripeAccount StripeAccount { get; set; }

        public TosVersion AcceptedTos { get; set; }
        public DateTime TosAcceptationDate { get; set; }
        public string TosAcceptationIpAddress { get; set; }
        public bool FirstLoginModalClosed { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
