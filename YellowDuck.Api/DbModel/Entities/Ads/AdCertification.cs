using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class AdCertification : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long AdId { get; set; }
        public Ad Ad { get; set; }
        public Certification Certification { get; set; }
    }
}
