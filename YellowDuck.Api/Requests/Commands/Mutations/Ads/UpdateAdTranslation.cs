using GraphQL.Conventions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.GraphTypes;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Plugins.GraphQL;
using YellowDuck.Api.Plugins.MediatR;

namespace YellowDuck.Api.Requests.Commands.Mutations.Ads
{
    public class UpdateAdTranslation : IRequestHandler<UpdateAdTranslation.Input, UpdateAdTranslation.Payload>
    {
        private readonly AppDbContext db;
        private readonly ILogger<UpdateAdTranslation> logger;

        public UpdateAdTranslation(AppDbContext db, ILogger<UpdateAdTranslation> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public async Task<Payload> Handle(Input request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            var adId = request.AdId.LongIdentifierForType<Ad>();

            var translation = await db.AdTranslations
                .FirstOrDefaultAsync(x => x.AdId == adId && x.Language == request.Language, cancellationToken);

            if (translation == null) throw new AdTranslationNotFoundException();

            request.Title.IfSet(v => translation.Title = v);
            request.Description.IfSet(v => translation.Description = v);
            request.RentPriceDescription.IfSet(v => translation.RentPriceDescription = v);
            request.SalePriceDescription.IfSet(v => translation.SalePriceDescription = v);
            request.TradeDescription.IfSet(v => translation.TradeDescription = v);
            request.DonationDescription.IfSet(v => translation.DonationDescription = v);
            request.Conditions.IfSet(v => translation.Conditions = v);
            request.DeliveryTruckTypeOther.IfSet(v => translation.DeliveryTruckTypeOther = v);
            request.Equipment.IfSet(v => translation.Equipment = v);
            request.SurfaceSize.IfSet(v => translation.SurfaceSize = v);
            request.ProfessionalKitchenEquipmentOther.IfSet(v => translation.ProfessionalKitchenEquipmentOther = v);
            request.SurfaceDescription.IfSet(v => translation.SurfaceDescription = v);

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Ad translation updated ({translation.Id})");

            return new Payload
            {
                AdTranslation = new AdTranslationGraphType(translation)
            };
        }

        private void ValidateRequest(Input request)
        {
            if (!request.Title.IsSet() && !request.Description.IsSet() && !request.RentPriceDescription.IsSet() && !request.SalePriceDescription.IsSet() && !request.DonationDescription.IsSet() && !request.TradeDescription.IsSet() && !request.Conditions.IsSet() && !request.SurfaceDescription.IsSet() && !request.Equipment.IsSet() && !request.SurfaceSize.IsSet() && !request.ProfessionalKitchenEquipmentOther.IsSet() && !request.DeliveryTruckTypeOther.IsSet())
            {
                throw new NothingToUpdateException();
            }

            if (request.Title.IsSet())
            {
                if (request.Title.Value == "")
                {
                    throw new EmptyTitleException();
                }
            }

            if (request.Description.IsSet())
            {
                if (request.Description.Value == "")
                {
                    throw new EmptyDescriptionException();
                }
            }

            if (request.RentPriceDescription.IsSet() && request.IsAvailableForRent.IsSet() && request.IsAvailableForRent.Value)
            {
                if (request.RentPriceDescription.Value == "" && (!request.RentPriceToBeDetermined.IsSet() || !request.RentPriceToBeDetermined.Value))
                {
                    throw new EmptyRentPriceDescriptionException();
                }
            }

            if (request.SalePriceDescription.IsSet() && request.IsAvailableForSale.IsSet() && request.IsAvailableForSale.Value)
            {
                if (request.SalePriceDescription.Value == "" && (!request.SalePriceToBeDetermined.IsSet() || !request.SalePriceToBeDetermined.Value))
                {
                    throw new EmptySalePriceDescriptionException();
                }
            }

            if (request.DonationDescription.IsSet() && request.IsAvailableForDonation.IsSet() && request.IsAvailableForDonation.Value)
            {
                if (request.DonationDescription.Value == "")
                {
                    throw new EmptyDonationDescriptionException();
                }
            }

            if (request.TradeDescription.IsSet() && request.IsAvailableForTrade.IsSet() && request.IsAvailableForTrade.Value)
            {
                if (request.TradeDescription.Value == "")
                {
                    throw new EmptyTradeDescriptionException();
                }
            }

            if (request.Conditions.IsSet())
            {
                if (request.Conditions.Value == "")
                {
                    throw new EmptyConditionsException();
                }
            }

            if (request.SurfaceDescription.IsSet())
            {
                if (request.SurfaceDescription.Value == "")
                {
                    throw new EmptySurfaceDescriptionException();
                }
            }

            if (request.ProfessionalKitchenEquipmentOther.IsSet())
            {
                if (request.ProfessionalKitchenEquipmentOther.Value == "")
                {
                    throw new EmptyProfessionalKitchenEquipmentOtherException();
                }
            }

            if (request.Equipment.IsSet())
            {
                if (request.Equipment.Value == "")
                {
                    throw new EmptyEquipmentException();
                }
            }

            if (request.SurfaceSize.IsSet())
            {
                if (request.SurfaceSize.Value == "")
                {
                    throw new EmptySurfaceSizeException();
                }
            }

            if (request.DeliveryTruckTypeOther.IsSet())
            {
                if (request.DeliveryTruckTypeOther.Value == "")
                {
                    throw new EmptyDeliveryTruckTypeOtherException();
                }
            }
        }

        [MutationInput]
        public class Input : IRequest<Payload>, IHaveAdId
        {
            public Id AdId { get; set; }
            public ContentLanguage Language { get; set; }

            public Maybe<NonNull<string>> Title { get; set; }
            public Maybe<NonNull<string>> Description { get; set; }
            public Maybe<NonNull<string>> RentPriceDescription { get; set; }
            public Maybe<NonNull<string>> SalePriceDescription { get; set; }
            public Maybe<NonNull<string>> DonationDescription { get; set; }
            public Maybe<NonNull<string>> TradeDescription { get; set; }
            public Maybe<NonNull<string>> Conditions { get; set; }
            public Maybe<NonNull<string>> SurfaceDescription { get; set; }
            public Maybe<NonNull<string>> ProfessionalKitchenEquipmentOther { get; set; }
            public Maybe<NonNull<string>> Equipment { get; set; }
            public Maybe<NonNull<string>> SurfaceSize { get; set; }
            public Maybe<NonNull<string>> DeliveryTruckTypeOther { get; set; }
            public Maybe<bool> RentPriceToBeDetermined { get; set; }
            public Maybe<bool> SalePriceToBeDetermined { get; set; }
            public Maybe<bool> IsAvailableForRent { get; set; }
            public Maybe<bool> IsAvailableForSale { get; set; }
            public Maybe<bool> IsAvailableForTrade { get; set; }
            public Maybe<bool> IsAvailableForDonation { get; set; }

        }

        [MutationPayload]
        public class Payload
        {
            public AdTranslationGraphType AdTranslation { get; set; }
        }

        [InputType]
        public class AddressInput
        {
            public Maybe<NonNull<string>> StreetNumber { get; set; }
            public Maybe<NonNull<string>> Route { get; set; }
            public Maybe<NonNull<string>> Locality { get; set; }
            public Maybe<NonNull<string>> Raw { get; set; }
            public Maybe<NonNull<string>> PostalCode { get; set; }
            public Maybe<double> Latitude { get; set; }
            public Maybe<double> Longitude { get; set; }
            public Maybe<NonNull<string>> Neighborhood { get; set; }
            public Maybe<NonNull<string>> Sublocality { get; set; }
        }

        public abstract class UpdateAdTranslationException : RequestValidationException { }

        public class AdTranslationNotFoundException : UpdateAdTranslationException { }
        public class NothingToUpdateException : UpdateAdTranslationException { }
        public class EmptyTitleException : UpdateAdTranslationException { }
        public class EmptyDescriptionException : UpdateAdTranslationException { }
        public class EmptyRentPriceDescriptionException : UpdateAdTranslationException { }
        public class EmptySalePriceDescriptionException : UpdateAdTranslationException { }

        public class EmptyDonationDescriptionException : UpdateAdTranslationException { }

        public class EmptyTradeDescriptionException : UpdateAdTranslationException { }

        public class EmptyConditionsException : UpdateAdTranslationException { }
        public class EmptySurfaceDescriptionException : UpdateAdTranslationException { }
        public class EmptyProfessionalKitchenEquipmentOtherException : UpdateAdTranslationException { }
        public class EmptyEquipmentException : UpdateAdTranslationException { }
        public class EmptySurfaceSizeException : UpdateAdTranslationException { }
        public class EmptyDeliveryTruckTypeOtherException : UpdateAdTranslationException { }
    }
}
