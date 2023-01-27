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
        public Task<string> PriceDescription => WithData(x => x.PriceDescription);
        public Task<string> Conditions => WithData(x => x.Conditions);
        public Task<string> SurfaceDescription => WithData(x => x.SurfaceDescription);
        public Task<string> ProfessionalKitchenEquipmentOther => WithData(x => x.ProfessionalKitchenEquipmentOther);
        public Task<string> Equipment => WithData(x => x.Equipment);
        public Task<string> SurfaceSize => WithData(x => x.SurfaceSize);
        public Task<string> DeliveryTruckTypeOther => WithData(x => x.DeliveryTruckTypeOther);
    }
}
