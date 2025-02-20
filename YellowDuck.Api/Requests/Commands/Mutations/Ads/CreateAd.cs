using GraphQL.Conventions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Helpers;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;
using YellowDuck.Api.Services.Files;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ads
{
    public class CreateAd : IRequestHandler<CreateAd.Input, CreateAd.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<CreateAd> logger;
        private readonly UserManager<AppUser> userManager;
        private readonly IFileManager fileManager;
        private readonly ICurrentUserAccessor currentUserAccessor;

        public CreateAd(AppDbContext db, UserManager<AppUser> userManager, ILogger<CreateAd> logger, IFileManager fileManager, ICurrentUserAccessor currentUserAccessor)
        {
            this.db = db;
            this.logger = logger;
            this.userManager = userManager;
            this.fileManager = fileManager;
            this.currentUserAccessor = currentUserAccessor;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request);
            var address = request.Address.Value;

            var ad = new Ad
            {
                CreatedAtUTC = DateTime.UtcNow,
                Category = request.Category,
                IsAvailableForRent = request.IsAvailableForRent,
                IsAvailableForSale = request.IsAvailableForSale,
                IsAvailableForTrade = request.IsAvailableForTrade,
                IsAvailableForDonation = request.IsAvailableForDonation,
                Translations = new List<AdTranslation>()
                {
                    new AdTranslation()
                    {
                        Language = request.Language,
                        Title = request.Title,
                        RentPriceDescription = request.RentPriceDescription,
                        SalePriceDescription = request.SalePriceDescription,
                        DonationDescription = request.DonationDescription,
                        TradeDescription = request.TradeDescription,
                        Conditions = "",
                        SurfaceDescription = "",
                        ProfessionalKitchenEquipmentOther = "",
                        Equipment = "",
                        SurfaceSize = "",
                        DeliveryTruckTypeOther = "",
                    }
                },
                RentPriceToBeDetermined = request.RentPriceToBeDetermined,
                RentPrice = request.RentPrice,
                SalePriceToBeDetermined = request.SalePriceToBeDetermined,
                SalePrice = request.SalePrice,
                Address = new AdAddress()
                {
                    Raw = address.Raw,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude
                },
                ShowAddress = request.ShowAddress,
                Organization = request.Organization,
                IsPublish = true
            };

            request.RentPriceRange.IfSet(v => ad.RentPriceRange = v);
            request.SalePriceRange.IfSet(v => ad.SalePriceRange = v);
            request.Address.Value.Locality.IfSet(v => ad.Address.Locality = v);
            request.Address.Value.PostalCode.IfSet(v => ad.Address.PostalCode = v);
            request.Address.Value.Route.IfSet(v => ad.Address.Route = v);
            request.Address.Value.StreetNumber.IfSet(v => ad.Address.StreetNumber = v);
            request.Address.Value.Neighborhood.IfSet(v => ad.Address.Neighborhood = v);
            request.Address.Value.Sublocality.IfSet(v => ad.Address.Sublocality = v);
            request.Description.IfSet(v => ad.Translations.First().Description = v);
            request.Conditions.IfSet(v => ad.Translations.First().Conditions = v);
            request.DeliveryTruckTypeOther.IfSet(v => ad.Translations.First().DeliveryTruckTypeOther = v);
            request.Equipment.IfSet(v => ad.Translations.First().Equipment = v);
            request.ProfessionalKitchenEquipmentOther.IfSet(v => ad.Translations.First().ProfessionalKitchenEquipmentOther = v);
            request.SurfaceDescription.IfSet(v => ad.Translations.First().SurfaceDescription = v);
            request.SurfaceSize.IfSet(v => ad.Translations.First().SurfaceSize = v);
            request.DeliveryTruckType.IfSet(v => ad.DeliveryTruckType = v);
            request.Refrigerated.IfSet(v => ad.Refrigerated = v);
            request.CanSharedRoad.IfSet(v => ad.CanSharedRoad = v);
            request.CanHaveDriver.IfSet(v => ad.CanHaveDriver = v);


            request.GalleryItems.IfSet(x =>
            {
                ad.Gallery = new List<AdGalleryItem>();
                x.Value.ForEach(y => ad.Gallery.Add(new AdGalleryItem() { PictureFileId = y.Src, Alt = y.Alt }));
            });

            request.DayAvailability.IfSet(v =>
            {
                ad.DayAvailability = new List<AdDayAvailability>();
                v.ForEach(x => ad.DayAvailability.Add(new AdDayAvailability() { Weekday = x }));
            });

            request.EveningAvailability.IfSet(v =>
            {
                ad.EveningAvailability = new List<AdEveningAvailability>();
                v.ForEach(x => ad.EveningAvailability.Add(new AdEveningAvailability() { Weekday = x }));
            });

            request.Certification.IfSet(v =>
            {
                ad.Certifications = new List<AdCertification>();
                v.ForEach(x => ad.Certifications.Add(new AdCertification() { Certification = x }));
            });

            request.ProfessionalKitchenEquipment.IfSet(v =>
            {
                ad.ProfessionalKitchenEquipments = new List<AdProfessionalKitchenEquipment>();
                v.ForEach(x => ad.ProfessionalKitchenEquipments.Add(new AdProfessionalKitchenEquipment() { ProfessionalKitchenEquipment = x }));
            });

            var owner = await currentUserAccessor.GetCurrentUser();
            ad.UserId = owner.Id;

            if(owner.Type == UserType.Admin)
            {
                ad.IsAdminOnly = true;
            }

            db.Ads.Add(ad);

            await db.SaveChangesAsync(cancellationToken);
            await userManager.AddClaimAsync(owner, new Claim(AppClaimTypes.AdOwner, Id.New<Ad>(ad.Id.ToString()).ToString()));

            logger.LogInformation($"New ad created {request.Title} ({ad.Id})");

            return new Payload
            {
                Ad = new AdGraphType(ad)
            };
        }

        private async Task ValidateRequest(Input request)
        {
            try
            {
                if (request.GalleryItems.IsSet())
                {
                    var pictures = request.GalleryItems.Value;
                    for (var i = 0; i < pictures.Value.Count; i++)
                    {
                        await MutationHelper.CheckPictureMaybe(pictures.Value[i].Src, fileManager);
                    }
                }
            }
            catch 
            {
                throw new ImageNotFoundException("GalleryItems");
            }

            if (request.Address.Value.Latitude == 0.0d || request.Address.Value.Longitude == 0.0d)
            {
                throw new AddressInvalidException();
            }

            switch (request.Category)
            {
                case AdCategory.DeliveryTruck:
                    {
                        if (!request.DeliveryTruckType.IsSet())
                        {
                            throw new DeliveryTruckTypeInvalidException();
                        }
                        else if (request.DeliveryTruckType.Value == DeliveryTruckType.Other && !string.IsNullOrWhiteSpace(request.DeliveryTruckTypeOther.Value))
                        {
                            throw new DeliveryTruckTypeOtherInvalidException();
                        }
                        break;
                    }
                case AdCategory.ProfessionalKitchen:
                    {
                        if (!request.SurfaceDescription.IsSet())
                        {
                            throw new SurfaceDescriptionInvalidException();
                        }
                        if (!request.ProfessionalKitchenEquipment.IsSet() || request.ProfessionalKitchenEquipment.Value.Count == 0)
                        {
                            throw new ProfessionalKitchenEquipmentInvalidException();
                        }
                        break;
                    }
                case AdCategory.StorageSpace:
                    {
                        if (!request.SurfaceSize.IsSet())
                        {
                            throw new SurfaceSizeInvalidException();
                        }
                        if (!request.Description.IsSet() || string.IsNullOrWhiteSpace(request.Description.Value))
                        {
                            throw new DescriptionInvalidException();
                        }
                        if (!request.Equipment.IsSet() || string.IsNullOrWhiteSpace(request.Description.Value))
                        {
                            throw new EquipmentInvalidException();
                        }
                        break;
                    }
                case AdCategory.Other:
                    {
                        if (!request.Description.IsSet() || string.IsNullOrWhiteSpace(request.Description.Value))
                        {
                            throw new DescriptionInvalidException();
                        }
                        break;
                    }
            }
           
        }

        [MutationInput]
        public class Input : IRequest<Payload>
        {
            public ContentLanguage Language { get; set; }
            public AdCategory Category { get; set; }
            public bool IsAvailableForRent { get; set; }
            public bool IsAvailableForSale { get; set; }
            public bool IsAvailableForTrade { get; set; }
            public bool IsAvailableForDonation { get; set; }
            public Maybe<NonNull<List<GalleryItemInput>>> GalleryItems { get; set; }
            public NonNull<string> Title { get; set; }
            public Maybe<NonNull<string>> Description { get; set; }
            public NonNull<AddressInput> Address { get; set; }
            public bool ShowAddress { get; set; }
            public double? RentPrice { get; set; }
            public bool RentPriceToBeDetermined { get; set; }
            public double? SalePrice { get; set; }
            public bool SalePriceToBeDetermined { get; set; }
            public string RentPriceDescription { get; set; }
            public string SalePriceDescription { get; set; }
            public Maybe<PriceRangeRental> RentPriceRange { get; set; }
            public Maybe<PriceRangeSale> SalePriceRange { get; set; }
            public string DonationDescription { get; set; }
            public string TradeDescription { get; set; }
            public Maybe<List<DayOfWeek>> DayAvailability { get; set; }
            public Maybe<List<DayOfWeek>> EveningAvailability { get; set; }
            public Maybe<List<Certification>> Certification { get; set; }
            public Maybe<string> Conditions { get; set; }
            public Maybe<NonNull<string>> SurfaceDescription { get; set; }
            public Maybe<List<ProfessionalKitchenEquipment>> ProfessionalKitchenEquipment { get; set; }
            public Maybe<NonNull<string>> ProfessionalKitchenEquipmentOther { get; set; }
            public Maybe<NonNull<string>> Equipment { get; set; }
            public Maybe<NonNull<string>> DeliveryTruckTypeOther { get; set; }
            public NonNull<string> Organization { get; set; }
            public Maybe<string> SurfaceSize { get; set; }
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

        public abstract class CreateAdException : RequestValidationException { }

        public class AddressInvalidException : CreateAdException { }
        public class DeliveryTruckTypeInvalidException : CreateAdException { }
        public class DeliveryTruckTypeOtherInvalidException : CreateAdException { }
        public class SurfaceDescriptionInvalidException : CreateAdException { }
        public class ProfessionalKitchenEquipmentInvalidException : CreateAdException { }
        public class SurfaceSizeInvalidException : CreateAdException { }
        public class DescriptionInvalidException : CreateAdException { }
        public class EquipmentInvalidException : CreateAdException { }

        public class ImageNotFoundException : CreateAdException
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
