using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Profiles
{
    public class UserProfile
    {
        public UserProfile() { }

        public UserProfile(UserProfile copyFrom)
        {
            if (copyFrom == null) return;
            FirstName = copyFrom.FirstName;
            LastName = copyFrom.LastName;
        }

        public long Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PublicName { get { return $"{FirstName} {LastName.Substring(0, 1)}."; } }

        public DateTime UpdateTimeUtc { get; set; }

        public string OrganizationName { get; set; }

        public string PostalCode { get; set; }

        public string PhoneNumber { get; set; }

        public bool ShowPhoneNumber { get; set; }

        public bool ShowEmail { get; set; }

        public bool ContactAuthorizationSurveys { get; set; }

        public bool ContactAuthorizationNews { get; set; }

        public OrganizationType OrganizationType { get; set; }
        public string OrganizationTypeOtherSpecification { get; set; }

        public Industry Industry { get; set; }
        public string IndustryOtherSpecification { get; set; }

        public IList<UserProfileRegisteringInterest> RegisteringInterests { get; set; }
    }
}