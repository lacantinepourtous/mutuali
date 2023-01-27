using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class AdProfessionalKitchenEquipment : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long AdId { get; set; }
        public Ad Ad { get; set; }
        public ProfessionalKitchenEquipment ProfessionalKitchenEquipment { get; set; }
    }
}
