using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Alerts
{
    public class AlertProfessionalKitchenEquipment : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long AlertId { get; set; }
        public Alert Alert { get; set; }
        public ProfessionalKitchenEquipment ProfessionalKitchenEquipment { get; set; }
    }
}
