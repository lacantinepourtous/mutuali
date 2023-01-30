using GraphQL.Conventions;
using System.Collections.Generic;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.Gql.Interfaces;
using System.Linq;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Services.Files;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.DbModel.Entities;
using MediatR;
using YellowDuck.Api.Requests.Queries.Rating;
using System;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class AdGraphType : LazyGraphType<Ad>
    {
        private readonly long id;

        public AdGraphType(IAppUserContext ctx, long adId) : base(() => ctx.LoadAd(adId))
        {
            id = adId;
        }

        public AdGraphType(Ad ad) : base(ad)
        {
            id = ad.Id;
        }

        public Id Id => Id.New<Ad>(id);

        public async Task<UserGraphType> User(IAppUserContext ctx)
        {
            var data = await Data;

            return data.User != null  
                ? new UserGraphType(data.User)
                : new UserGraphType(ctx, data.UserId);
        }

        public Task<AdCategory> Category => WithData(x => x.Category);

        public Task<double?> Price => WithData(x => x.Price);

        public Task<bool> PriceToBeDetermined => WithData(x => x.PriceToBeDetermined);

        public Task<bool> ShowAddress => WithData(x => x.ShowAddress);

        public async Task<IEnumerable<DayOfWeek>> DayAvailability(IAppUserContext ctx)
        {
            var dayAvailability = await ctx.LoadDayAvailabilityByAdId(id);
            return dayAvailability.Select(x => x.Weekday).ToList();
        }
        public async Task<IEnumerable<DayOfWeek>> EveningAvailability(IAppUserContext ctx)
        {
            var eveningAvailability = await ctx.LoadEveningAvailabilityByAdId(id);
            return eveningAvailability.Select(x => x.Weekday).ToList();
        }
        public async Task<IEnumerable<ProfessionalKitchenEquipment>> ProfessionalKitchenEquipment(IAppUserContext ctx)
        {
            var professionalKitchenEquipments = await ctx.LoadProfessionalKitchenEquipmentsByAdId(id);
            return professionalKitchenEquipments.Select(x => x.ProfessionalKitchenEquipment).ToList();
        }
        public Task<DeliveryTruckType> DeliveryTruckType => WithData(x => x.DeliveryTruckType);

        public Task<bool> Refrigerated => WithData(x => x.Refrigerated);

        public Task<bool> CanSharedRoad => WithData(x => x.CanSharedRoad);

        public Task<bool> CanHaveDriver => WithData(x => x.CanHaveDriver);

        public async Task<IEnumerable<AdGalleryItemGraphType>> Gallery(IAppUserContext ctx)
        {
            var galleryItems = await ctx.LoadAdGalleryItems(id);
            return galleryItems.Select(x => new AdGalleryItemGraphType(x)).ToList();
        }

        public async Task<AdAddressGraphType> Address(IAppUserContext ctx)
        {
            var data = await Data;

            return data.Address != null
                ? new AdAddressGraphType(data.Address)
                : new AdAddressGraphType(ctx, data.AddressId);
        }

        public async Task<AdTranslationGraphType> TranslationOrDefault(IAppUserContext ctx, ContentLanguage language)
        {
            var translations = await ctx.LoadAdTranslations(id);
            return translations
                .OrderBy(x => x.Language == language ? 0 : 1)
                .ThenBy(x => x.Language)
                .Select(x => new AdTranslationGraphType(x))
                .First();
        }

        public async Task<IEnumerable<AdTranslationGraphType>> Translations(IAppUserContext ctx)
        {
            var translations = await ctx.LoadAdTranslations(id);
            return translations.OrderBy(t => t.Language).Select(x => new AdTranslationGraphType(x)).ToList();
        }

        public async Task<IEnumerable<AdRatingGraphType>> AdRatings(IAppUserContext ctx)
        {
            var adRatings = await ctx.LoadAdRatingByAdId(id);
            return adRatings.Select(x => new AdRatingGraphType(x)).ToList();
        }

        public async Task<double> AverageRating([Inject] IMediator mediator)
        {
            return await mediator.Send(new GetAdAverageRating.Query
            {
                AdId = id
            });
        }

        public Task<string> Organization => WithData(x => x.Organization);

        public Task<bool> IsPublish => WithData(x => x.IsPublish);

        public Task<DateTime?> CreatedAtUTC => WithData(x => x.CreatedAtUTC);
    }
}
