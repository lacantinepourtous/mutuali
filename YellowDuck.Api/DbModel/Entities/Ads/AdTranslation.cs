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
        public string Conditions { get; set; }

        // Transaction types descriptions
        public string RentPriceDescription { get; set; }
        public string SalePriceDescription { get; set; }
        public string DonationDescription { get; set; }
        public string TradeDescription { get; set; }

        // Professionnal Kitchen
        public string SurfaceDescription { get; set; }
        public string ProfessionalKitchenEquipmentOther { get; set; }

        // Storage Space
        public string Equipment { get; set; }
        public string SurfaceSize { get; set; }

        // Delivery Truck
        public string DeliveryTruckTypeOther { get; set; }

        // Human Resources
        public string HumanResourceFieldOther { get; set; }
        public string Qualifications { get; set; }
        public string Tasks { get; set; }
        public string GeographicCoverage { get; set; }
    }
}
