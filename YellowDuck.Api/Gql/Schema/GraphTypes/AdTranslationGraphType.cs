using GraphQL.Conventions;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class AdTranslationGraphType : LazyGraphType<AdTranslation>
    {
        public AdTranslationGraphType(AdTranslation translation) : base(translation)
        {
            Id = translation.GetIdentifier();
        }

        public Id Id { get; }

        public Task<ContentLanguage> Language => WithData(x => x.Language);
        public Task<string> Title => WithData(x => x.Title);
        public Task<string> Description => WithData(x => x.Description);
        public Task<string> RentPriceDescription => WithData(x => x.RentPriceDescription);
        public Task<string> SalePriceDescription => WithData(x => x.SalePriceDescription);
        public Task<string> DonationDescription => WithData(x => x.DonationDescription);
        public Task<string> TradeDescription => WithData(x => x.TradeDescription);
        public Task<string> Conditions => WithData(x => x.Conditions);
        public Task<string> SurfaceDescription => WithData(x => x.SurfaceDescription);
        public Task<string> ProfessionalKitchenEquipmentOther => WithData(x => x.ProfessionalKitchenEquipmentOther);
        public Task<string> Equipment => WithData(x => x.Equipment);
        public Task<string> SurfaceSize => WithData(x => x.SurfaceSize);
        public Task<string> DeliveryTruckTypeOther => WithData(x => x.DeliveryTruckTypeOther);
        public Task<string> HumanResourceFieldOther => WithData(x =>x.HumanResourceFieldOther);
        public Task<string> Tasks => WithData(x => x.Tasks);
        public Task<string> Qualifications => WithData(x => x.Qualifications);
        public Task<string> GeographicCoverage => WithData(x => x.GeographicCoverage);
    }
}
