using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Helpers;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.Files;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ads
{
    public class UpdateAd : IRequestHandler<UpdateAd.Input, UpdateAd.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<UpdateAd> logger;
        private readonly IFileManager fileManager;

        public UpdateAd(AppDbContext db, ILogger<UpdateAd> logger, IFileManager fileManager)
        {
            this.db = db;
            this.logger = logger;
            this.fileManager = fileManager;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);

            var adId = request.AdId.LongIdentifierForType<Ad>();

            var ad = await db.Ads
                .Include(x => x.Address)
                .Include(x => x.Gallery)
                .FirstOrDefaultAsync(x => x.Id == adId, cancellationToken);

            if (ad == null) throw new AdNotFoundException();
            if(ad.Category == AdCategory.ProfessionalKitchen) ValidateProfessionalKitchenRequest(request);

            request.Category.IfSet(v => ad.Category = v);
            request.Address.IfSet(v => UpdateAddress(ad.Address, v));
            request.ShowAddress.IfSet(v => ad.ShowAddress = v);
            request.GalleryItems.IfSet(v => UpdateGalleryItems(ad, v.Value));
            request.Price.IfSet(v => ad.Price = v);
            request.PriceToBeDetermined.IfSet(v => ad.PriceToBeDetermined = v);
            request.Organization.IfSet(v => ad.Organization = v);
            request.DayAvailability.IfSet( v => UpdateDayAvailability(ad, v));
            request.EveningAvailability.IfSet(v => UpdateEveningAvailability(ad, v));
            request.ProfessionalKitchenEquipment.IfSet(v => UpdateProfessionalKitchenEquipments(ad, v));
            request.DeliveryTruckType.IfSet(v => ad.DeliveryTruckType = v);
            request.Refrigerated.IfSet(v => ad.Refrigerated = v);
            request.CanSharedRoad.IfSet(v => ad.CanSharedRoad = v);
            request.CanHaveDriver.IfSet(v => ad.CanHaveDriver = v);

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Ad updated ({ad.Id})");

            return new Payload
            {
                Ad = new AdGraphType(ad)
            };
        }

        private void ResetAddress(AdAddress address)
        {
            address.Latitude = 0f;
            address.Longitude = 0f;
            address.Locality = "";
            address.Neighborhood = "";
            address.PostalCode = "";
            address.Raw = "";
            address.Route = "";
            address.StreetNumber = "";
            address.Sublocality = "";
        }

        private void UpdateAddress(AdAddress address, AddressInput input)
        {
            ResetAddress(address);

            address.Latitude = input.Latitude;
            address.Longitude = input.Longitude;
            address.Raw = input.Raw;

            input.Neighborhood.IfSet(v => address.Neighborhood = v);
            input.Locality.IfSet(v => address.Locality = v);
            input.PostalCode.IfSet(v => address.PostalCode = v);
            input.Route.IfSet(v => address.Route = v);
            input.StreetNumber.IfSet(v => address.StreetNumber = v);
            input.Sublocality.IfSet(v => address.Sublocality = v);
        }

        private void UpdateGalleryItems(Ad ad, List<GalleryItemInput> galleryItems)
        {
            ad.Gallery = new List<AdGalleryItem>();
            galleryItems.ForEach(y => ad.Gallery.Add(new AdGalleryItem() { PictureFileId = y.Src, Alt = y.Alt }));
        }

        private void UpdateDayAvailability(Ad ad, List<DayOfWeek> dayAvailability)
        {
            db.AdDayAvailabilityWeekdays.RemoveRange(db.AdDayAvailabilityWeekdays.Where(x => x.AdId == ad.Id));
            ad.DayAvailability = new List<AdDayAvailability>();
            dayAvailability.ForEach(x => ad.DayAvailability.Add(new AdDayAvailability() { Weekday = x }));
        }

        private void UpdateEveningAvailability(Ad ad, List<DayOfWeek> eveningAvailability)
        {
            db.AdEveningAvailabilityWeekdays.RemoveRange(db.AdEveningAvailabilityWeekdays.Where(x => x.AdId == ad.Id));
            ad.EveningAvailability = new List<AdEveningAvailability>();
            eveningAvailability.ForEach(x => ad.EveningAvailability.Add(new AdEveningAvailability() { Weekday = x }));
        }

        private void UpdateProfessionalKitchenEquipments(Ad ad, List<ProfessionalKitchenEquipment> professionalKitchenEquipments)
        {
            db.AdProfessionalKitchenEquipments.RemoveRange(db.AdProfessionalKitchenEquipments.Where(x => x.AdId == ad.Id));
            ad.ProfessionalKitchenEquipments = new List<AdProfessionalKitchenEquipment>();
            professionalKitchenEquipments.ForEach(x => ad.ProfessionalKitchenEquipments.Add(new AdProfessionalKitchenEquipment() { ProfessionalKitchenEquipment = x }));
        }

        private async Task ValidateRequest(Input request)
        {
            try
            {
                if (request.GalleryItems.IsSet())
                {
                    var pictures = request.GalleryItems.Value;
                    foreach (var picture in pictures.Value)
                    {
                        await MutationHelper.CheckPictureMaybe(picture.Src, fileManager);
                    }
                }
            }
            catch
            {
                throw new ImageNotFoundException("GalleryItems");
            }
        }

        private void ValidateProfessionalKitchenRequest(Input request)
        {
            if (request.ProfessionalKitchenEquipment.IsSet() && request.ProfessionalKitchenEquipment.Value.Count == 0)
            {
                throw new ProfessionalKitchenEquipmentInvalidException();
            }
        }

        [MutationInput]
        public class Input : IRequest<Payload>, IHaveAdId
        {
            public Id AdId { get; set; }
            public Maybe<AdCategory> Category { get; set; }
            public Maybe<NonNull<List<GalleryItemInput>>> GalleryItems { get; set; }
            public Maybe<AddressInput> Address { get; set; }
            public Maybe<bool> ShowAddress { get; set; }
            public Maybe<List<DayOfWeek>> DayAvailability { get; set; }
            public Maybe<List<DayOfWeek>> EveningAvailability { get; set; }
            public Maybe<double?> Price { get; set; }
            public Maybe<bool> PriceToBeDetermined { get; set; }
            public Maybe<string> Organization { get; set; }
            public Maybe<List<ProfessionalKitchenEquipment>> ProfessionalKitchenEquipment { get; set; }
            public Maybe<DeliveryTruckType> DeliveryTruckType { get; set; }
            public Maybe<bool> Refrigerated { get; set; }
            public Maybe<bool> CanSharedRoad { get; set; }
            public Maybe<bool> CanHaveDriver { get; set; }
        }

        [MutationPayload]
        public class Payload
        {
            public AdGraphType Ad { get; set; }
        }

        [InputType]
        public class AddressInput
        {
            public Maybe<NonNull<string>> StreetNumber { get; set; }
            public Maybe<NonNull<string>> Route { get; set; }
            public Maybe<NonNull<string>> Locality { get; set; }
            public NonNull<string> Raw { get; set; }
            public Maybe<NonNull<string>> PostalCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public Maybe<NonNull<string>> Neighborhood { get; set; }
            public Maybe<NonNull<string>> Sublocality { get; set; }
        }

        [InputType]
        public class GalleryItemInput
        {
            public string Src { get; set; }
            public string Alt { get; set; }
        }

        public abstract class EditAdException : RequestValidationException { }

        public class AdNotFoundException : EditAdException { }
        public class ProfessionalKitchenEquipmentInvalidException : EditAdException { }

        public class ImageNotFoundException : EditAdException
        {
            private readonly string propName;

            public ImageNotFoundException(string propName)
            {
                this.propName = propName;
            }

            public override IDictionary Data => new Dictionary<string, string> {
                {"Property", propName}
            };
        }
    }
}
