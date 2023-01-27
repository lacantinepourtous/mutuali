using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Profiles
{
    public class UserProfileRegisteringInterest : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public RegisteringInterest RegisteringInterest { get; set; }
    }
}
