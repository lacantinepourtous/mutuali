using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class AdTranslation : IHaveLongIdentifier
    {
        public long Id { get; set; }

        public long AdId { get; set; }
        public Ad Ad { get; set; }

        public ContentLanguage Language { get; set; }

        // General
        public string Title { get; set; }
        public string Description { get; set; }
        public string PriceDescription { get; set; }
        public string Conditions { get; set; }

        // Professionnal Kitchen
        public string SurfaceDescription { get; set; }
        public string ProfessionalKitchenEquipmentOther { get; set; }

        // Storage Space
        public string Equipment { get; set; }
        public string SurfaceSize { get; set; }

        // Delivery Truck
        public string DeliveryTruckTypeOther { get; set; }
    }
}
