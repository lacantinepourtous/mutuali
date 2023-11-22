﻿using System;
using System.Collections.Generic;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class Ad : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public IList<AdTranslation> Translations { get; set; }

        // General informations
        public IList<AdGalleryItem> Gallery { get; set; }
        public AdCategory Category { get; set; }
        public string Organization { get; set; }
        public IList<AdDayAvailability> DayAvailability { get; set; }
        public IList<AdEveningAvailability> EveningAvailability { get; set; }

        // Address
        public long AddressId { get; set; }
        public AdAddress Address { get; set; }
        public bool ShowAddress { get; set; }

        // Price
        public double? Price { get; set; }
        public bool PriceToBeDetermined { get; set; }
        public IList<AdRating> AdRatings { get; set; }

        // Professional Kitchen
        public IList<AdProfessionalKitchenEquipment> ProfessionalKitchenEquipments { get; set; }

        // Delivery Truck
        public DeliveryTruckType DeliveryTruckType { get; set; }
        public bool Refrigerated { get; set; }
        public bool CanSharedRoad { get; set; }
        public bool CanHaveDriver { get; set; }

        public bool IsPublish { get; set; }
        public bool IsAdminOnly { get; set; }
        public DateTime? CreatedAtUTC { get; set; }
    }
}
