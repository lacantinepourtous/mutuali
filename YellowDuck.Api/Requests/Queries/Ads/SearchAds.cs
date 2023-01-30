using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.Utilities;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.Utilities.Sorting;
using System.Collections.Generic;
using YellowDuck.Api.DbModel.Entities.Ads;
using Microsoft.EntityFrameworkCore;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.DbModel.Enums;

namespace YellowDuck.Api.Requests.Queries.Ads
{
    public class SearchAds : IRequestHandler<SearchAds.Query, IEnumerable<Ad>>
    {
        private readonly AppDbContext db;

        public SearchAds(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<Ad>> Handle(Query query, CancellationToken cancellationToken)
        {
            IQueryable<Ad> adsQuery = db.Ads.AsQueryable();

            if (query.Category != null)
            {
                adsQuery = adsQuery.Where(x => x.Category == query.Category);
            }

            if (query.DeliveryTruckType != null)
            {
                adsQuery = adsQuery.Where(x => x.DeliveryTruckType == query.DeliveryTruckType);
            }

            if (query.Refrigerated != null)
            {
                adsQuery = adsQuery.Where(x => x.Refrigerated == query.Refrigerated);
            }

            if (query.CanHaveDriver != null)
            {
                adsQuery = adsQuery.Where(x => x.CanHaveDriver == query.CanHaveDriver);
            }

            if (query.CanSharedRoad != null)
            {
                adsQuery = adsQuery.Where(x => x.CanSharedRoad == query.CanSharedRoad);
            }

            if (query.DayAvailability?.Any() ?? false)
            {
                var dayAvailabilityAdIds = await db.AdDayAvailabilityWeekdays
                                                    .Where(x => query.DayAvailability.Contains(x.Weekday))
                                                    .GroupBy(x => x.AdId)
                                                    .Where(x => x.Count() == query.DayAvailability.Count)
                                                    .Select(x => x.Key)
                                                    .ToListAsync();
                adsQuery = adsQuery.Where(x => dayAvailabilityAdIds.Contains(x.Id));
            }

            if (query.EveningAvailability?.Any() ?? false)
            {
                var eveningAvailabilityAdIds = await db.AdEveningAvailabilityWeekdays
                                                    .Where(x => query.EveningAvailability.Contains(x.Weekday))
                                                    .GroupBy(x => x.AdId)
                                                    .Where(x => x.Count() == query.EveningAvailability.Count)
                                                    .Select(x => x.Key)
                                                    .ToListAsync();
                adsQuery = adsQuery.Where(x => eveningAvailabilityAdIds.Contains(x.Id));
            }

            if (query.ProfessionalKitchenEquipment?.Any() ?? false)
            {

                var professionalKitchenEquipmentsAdIds = await db.AdProfessionalKitchenEquipments
                                                    .Where(x => query.ProfessionalKitchenEquipment.Contains(x.ProfessionalKitchenEquipment))
                                                    .GroupBy(x => x.AdId)
                                                    .Where(x => x.Count() == query.ProfessionalKitchenEquipment.Count)
                                                    .Select(x => x.Key)
                                                    .ToListAsync();
                adsQuery = adsQuery.Where(x => professionalKitchenEquipmentsAdIds.Contains(x.Id));
            }

            var ads = await adsQuery.ToListAsync(cancellationToken: cancellationToken);
            return ads;
        }

        public class Query : IRequest<IEnumerable<Ad>>
        {
            public AdCategory? Category = null;
            public IList<DayOfWeek> DayAvailability = null;
            public IList<DayOfWeek> EveningAvailability = null;
            public IList<ProfessionalKitchenEquipment> ProfessionalKitchenEquipment = null;
            public DeliveryTruckType? DeliveryTruckType = null;
            public bool? Refrigerated = null;
            public bool? CanSharedRoad = null;
            public bool? CanHaveDriver = null;
        }

    }
}
