using FluentAssertions;
using GraphQL.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Requests.Commands.Mutations.Ads;
using YellowDuck.Api.Services.Files;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Ads
{
    public class CreateAdTests : TestBase
    {
        private readonly Mock<IFileManager> fileManager;
        private readonly CreateAd handler;
        private readonly AppUser user;

        public CreateAdTests()
        {
            fileManager = new Mock<IFileManager>();
            handler = new CreateAd(DbContext, UserManager, NullLogger<CreateAd>.Instance, fileManager.Object, UserAccessor);
            user = AddUser("test@example.com", UserType.User);
            SetLoggedInUser(user);
        }

        [Fact]
        public async Task ShouldAddTheAdToDb()
        {
            var input = new CreateAd.Input
            {
                Title = "Test ad",
                Category = AdCategory.ProfessionalKitchen,
                DayAvailability = new Maybe<List<DayOfWeek>>()
                {
                    Value = new List<DayOfWeek>()
                    {
                        DayOfWeek.Saturday,
                        DayOfWeek.Sunday
                    }
                },
                ProfessionalKitchenEquipment = new Maybe<List<ProfessionalKitchenEquipment>>()
                {
                    Value = new List<ProfessionalKitchenEquipment>()
                    {
                        ProfessionalKitchenEquipment.WORKPLAN,
                        ProfessionalKitchenEquipment.OTHER
                    }
                },
                ProfessionalKitchenEquipmentOther = new Maybe<NonNull<string>>("Example"),
                Description = new Maybe<NonNull<string>>("Test ad description"),
                SurfaceDescription = new Maybe<NonNull<string>>("Test ad surface description"),
                Language = ContentLanguage.French,
                Address = new CreateAd.AddressInput()
                {
                    StreetNumber = "123".NonNull(),
                    Route = "Example route".NonNull(),
                    Latitude = 46.8214225354811,
                    Longitude = -71.23779832297491,
                    Locality = "Québec".NonNull(),
                    PostalCode = "G1K-0H1".NonNull(),
                    Raw = "{raw object}"
                }.NonNull(),
                ShowAddress = true,
                DeliveryTruckType = DeliveryTruckType.CubeTruck12Foot,
            };

            await handler.Handle(input, CancellationToken.None);

            // Creating a new Db Context to verify that SaveChanges was called
            using (var db = CreateDbContext())
            {
                var ad = await db.Ads
                    .Include(x => x.Translations)
                    .Include(x => x.ProfessionalKitchenEquipments)
                    .Include(x => x.DayAvailability)
                    .FirstAsync();

                ad.Category.Should().Be(AdCategory.ProfessionalKitchen);
                ad.Gallery.Should().BeNull();
                ad.Translations.Should().HaveCount(1);
                ad.ShowAddress.Should().Be(true);
                ad.DeliveryTruckType.Should().Be(DeliveryTruckType.CubeTruck12Foot);
                ad.ProfessionalKitchenEquipments.Should().HaveCount(2);
                ad.DayAvailability.Should().Contain(x => x.Weekday == DayOfWeek.Saturday);
                ad.DayAvailability.Should().Contain(x => x.Weekday == DayOfWeek.Sunday);

                var french = await db.AdTranslations.FirstAsync();
                french.Title.Should().Be("Test ad");
                french.Description.Should().Be("Test ad description");
                french.SurfaceDescription.Should().Be("Test ad surface description");
                french.ProfessionalKitchenEquipmentOther.Should().Be("Example");
            }
        }

        [Fact]
        public async Task ShouldReturnTheCreatedAd()
        {
            var input = new CreateAd.Input
            {
                Title = "Test ad",
                Category = AdCategory.DeliveryTruck,
                DeliveryTruckType = DeliveryTruckType.Van,
                Language = ContentLanguage.French,
                Address = new CreateAd.AddressInput()
                {
                    StreetNumber = "123".NonNull(),
                    Route = "Example route".NonNull(),
                    Latitude = 46.8214225354811,
                    Longitude = -71.23779832297491,
                    Locality = "Québec".NonNull(),
                    PostalCode = "G1K-0H1".NonNull(),
                    Raw = "{raw object}"
                }.NonNull(),
                ShowAddress = true
            };

            var result = await handler.Handle(input, CancellationToken.None);

            var category = await result.Ad.Category;
            category.Should().Be(AdCategory.DeliveryTruck);
        }

        [Fact]
        public async Task ShouldReturnIdOfCreatedAd()
        {
            var input = new CreateAd.Input
            {
                Title = "Test ad",
                Category = AdCategory.DeliveryTruck,
                DeliveryTruckType = DeliveryTruckType.Van,
                Language = ContentLanguage.French,
                Address = new CreateAd.AddressInput()
                {
                    StreetNumber = "123".NonNull(),
                    Route = "Example route".NonNull(),
                    Latitude = 46.8214225354811,
                    Longitude = -71.23779832297491,
                    Locality = "Québec".NonNull(),
                    PostalCode = "G1K-0H1".NonNull(),
                    Raw = "{raw object}"
                }.NonNull(),
                ShowAddress= true
            };

            var result = await handler.Handle(input, CancellationToken.None);

            var adId = result.Ad.Id.LongIdentifierForType<Ad>();
            var ad = await DbContext.Ads.FindAsync(adId);

            ad.Category.Should().Be(AdCategory.DeliveryTruck);
        }

        [Fact]
        public async Task ValidatesExistenceOfPictures()
        {
            var input = new CreateAd.Input
            {
                Title = "Test ad",
                Category = AdCategory.DeliveryTruck,
                DeliveryTruckType = DeliveryTruckType.Van,
                Language = ContentLanguage.French,
                GalleryItems = (new List<CreateAd.GalleryItemInput>() { new CreateAd.GalleryItemInput{ Src = "test.png", Alt = "Texte" }  }).NonNull(),
                Address = new CreateAd.AddressInput()
                {
                    StreetNumber = "123".NonNull(),
                    Route = "Example route".NonNull(),
                    Latitude = 46.8214225354811,
                    Longitude = -71.23779832297491,
                    Locality = "Québec".NonNull(),
                    PostalCode = "G1K-0H1".NonNull(),
                    Raw = "{raw object}"
                }.NonNull(),
                ShowAddress = true
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<CreateAd.ImageNotFoundException>();

            fileManager.Setup(x => x.Exists(FileContainers.Images, "test.png"))
                .ReturnsAsync(true);

            var result = handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var ad = await db.Ads.Include(x => x.Gallery).FirstAsync();
                ad.Gallery.First().PictureFileId.Should().Be("test.png");
                ad.Gallery.First().Alt.Should().Be("Texte");
            }
        }

        [Fact]
        public async Task ValidateClaims()
        {
            var input = new CreateAd.Input
            {
                Title = "Test ad",
                Category = AdCategory.DeliveryTruck,
                DeliveryTruckType = DeliveryTruckType.Van,
                Language = ContentLanguage.French,
                Address = new CreateAd.AddressInput()
                {
                    StreetNumber = "123".NonNull(),
                    Route = "Example route".NonNull(),
                    Latitude = 46.8214225354811,
                    Longitude = -71.23779832297491,
                    Locality = "Québec".NonNull(),
                    PostalCode = "G1K-0H1".NonNull(),
                    Raw = "{raw object}"
                }.NonNull(),
                ShowAddress = true
            };

            var result = await handler.Handle(input, CancellationToken.None);

            var claim = new Claim(AppClaimTypes.AdOwner, result.Ad.Id.ToString());
            var assignedUsers = await UserManager.GetUsersForClaimAsync(claim);
            assignedUsers.Should().HaveCount(1)
                .And.Contain(x => x.Email == "test@example.com");
        }

        [Fact]
        public async Task ValidateMissingLattitudeLongitude()
        {
            var input = new CreateAd.Input
            {
                Title = "Test ad",
                Category = AdCategory.DeliveryTruck,
                Language = ContentLanguage.French,
                Address = new CreateAd.AddressInput()
                {
                    StreetNumber = "123".NonNull(),
                    Route = "Example route".NonNull(),
                    Locality = "Québec".NonNull(),
                    PostalCode = "G1K-0H1".NonNull(),
                    Raw = "{raw object}"
                }.NonNull(),
                ShowAddress = true
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<CreateAd.AddressInvalidException>();
        }
    }
}
