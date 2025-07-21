using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Requests.Commands.Mutations.Ads;
using YellowDuck.Api.Services.Files;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Ads
{
    public class UpdateAdTests : TestBase
    {
        private readonly Mock<IFileManager> fileManager;
        private readonly UpdateAd handler;
        private readonly AppUser user;
        private readonly Ad ad;

        public UpdateAdTests()
        {
            fileManager = new Mock<IFileManager>();
            handler = new UpdateAd(DbContext, NullLogger<UpdateAd>.Instance, fileManager.Object);
            user = AddUser("test@example.com", UserType.User);
            SetLoggedInUser(user);

            ad = new Ad()
            {
                Category = AdCategory.StorageSpace,
                DayAvailability = new List<AdDayAvailability>()
                {
                    new AdDayAvailability()
                    {
                        Weekday = DayOfWeek.Saturday
                    },
                    new AdDayAvailability()
                    {
                        Weekday = DayOfWeek.Sunday
                    },
                },
                Address = new AdAddress()
                {
                    Latitude = 46.8214098,
                    Longitude = -71.237595,
                    Locality = "Québec",
                    Neighborhood = "Saint-Roch",
                    Sublocality = "La Cité-Limoilou",
                    PostalCode = "G1K0H1",
                    Raw = "{\"street_number\":\"395\",\"route\":\"Rue Bickell\",\"locality\":\"Québec\",\"administrative_area_level_2\":\"Communauté - Urbaine - de - Québec\",\"administrative_area_level_1\":\"QC\",\"country\":\"Canada\",\"latitude\":46.8214098,\"longitude\":-71.237595,\"neighborhood\":\"Saint - Roch\",\"sublocality\":\"La Cité - Limoilou\"}",
                    Route = "Rue Bickell",
                    StreetNumber = "395"
                },
                Translations = new List<AdTranslation>()
                {
                    new AdTranslation()
                    {
                        Title = "Titre test 1",
                        Description = "<h3>Ceci est une description de test</h3>",
                        Language = ContentLanguage.French
                    }
                },
                Gallery = new List<AdGalleryItem>()
                {
                    new AdGalleryItem()
                    {
                        PictureFileId = "cbecdbac-33cf-4268-b0da-fbbbb11c17a5/favicon.PNG",
                        Alt = "Alt texte"
                    },
                    new AdGalleryItem()
                    {
                        PictureFileId = "27fa655d-56f3-4ab3-8318-59272328625f/favicon.PNG",
                        Alt = "Texte alternatif"
                    }
                }
            };

            DbContext.Ads.Add(ad);
            DbContext.SaveChanges();
        }

        private UpdateAd.Input GetValidInput()
        {
            var input = new UpdateAd.Input
            {
                AdId = ad.GetIdentifier(),
                Category = AdCategory.ProfessionalKitchen,
                ProfessionalKitchenEquipment = new List<ProfessionalKitchenEquipment>()
                {
                    ProfessionalKitchenEquipment.WORKPLAN,
                    ProfessionalKitchenEquipment.OVEN
                },
                DayAvailability = new List<DayOfWeek>()
                {
                    DayOfWeek.Monday
                },
                Address = new UpdateAd.AddressInput()
                {
                    Latitude = 45.5576387,
                    Longitude = -73.5515693,
                    Locality = "Montréal".NonNull(),
                    Neighborhood = "".NonNull(),
                    PostalCode = "H2G 2B3".NonNull(),
                    Raw = "{\"street_number\":\"5650\",\"route\":\"Rue D'Iberville\",\"locality\":\"Montréal\",\"administrative_area_level_2\":\"Communauté-Urbaine-de-Montréal\",\"administrative_area_level_1\":\"QC\",\"country\":\"Canada\",\"postal_code\":\"H2G 2B3\",\"latitude\":45.544332,\"longitude\":-73.58167399999999}".NonNull(),
                    Route = "Rue D'Iberville".NonNull(),
                    StreetNumber = "5650".NonNull(),
                    Sublocality = "Rosemont-La Petite-Patrie".NonNull()
                },
                Refrigerated = true,
                CanHaveDriver = true,
                CanSharedRoad = false,
                HumanResourceField = HumanResourceField.Maintenance
            };
            return input;
        }

        [Fact]
        public async Task ShouldUpdateAllFields()
        {
            var input = GetValidInput();
            await handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var ad = await db.Ads
                    .Include(x => x.Address)
                    .Include(x => x.ProfessionalKitchenEquipments)
                    .Include(x => x.DayAvailability)
                    .FirstAsync();

                ad.Category.Should().Be(AdCategory.ProfessionalKitchen);
                ad.Address.Latitude.Should().Be(45.5576387);
                ad.Address.Longitude.Should().Be(-73.5515693);
                ad.Address.Locality.Should().Be("Montréal");
                ad.Address.Neighborhood.Should().Be("");
                ad.Address.PostalCode.Should().Be("H2G 2B3");
                ad.Address.Raw.Should().Be("{\"street_number\":\"5650\",\"route\":\"Rue D'Iberville\",\"locality\":\"Montréal\",\"administrative_area_level_2\":\"Communauté-Urbaine-de-Montréal\",\"administrative_area_level_1\":\"QC\",\"country\":\"Canada\",\"postal_code\":\"H2G 2B3\",\"latitude\":45.544332,\"longitude\":-73.58167399999999}");
                ad.Address.Route.Should().Be("Rue D'Iberville");
                ad.Address.StreetNumber.Should().Be("5650");
                ad.Address.Sublocality.Should().Be("Rosemont-La Petite-Patrie");
                ad.ProfessionalKitchenEquipments.Should().Contain(x => x.ProfessionalKitchenEquipment == ProfessionalKitchenEquipment.WORKPLAN);
                ad.ProfessionalKitchenEquipments.Should().Contain(x => x.ProfessionalKitchenEquipment == ProfessionalKitchenEquipment.OVEN);
                ad.DayAvailability.Should().Contain(x => x.Weekday == DayOfWeek.Monday);
                ad.DayAvailability.Should().NotContain(x => x.Weekday == DayOfWeek.Sunday);
                ad.Refrigerated.Should().Be(true);
                ad.CanHaveDriver.Should().Be(true);
                ad.CanSharedRoad.Should().Be(false);
                ad.HumanResourceField.Should().Be(HumanResourceField.Maintenance);
            }
        }

        [Fact]
        public async Task ShouldReturnTheUpdatedAd()
        {
            var input = GetValidInput();
            var result = await handler.Handle(input, CancellationToken.None);

            result.Ad.Should().NotBeNull();

            var category = await result.Ad.Category;
            category.Should().Be(AdCategory.ProfessionalKitchen);

            var id = result.Ad.Id.LongIdentifierForType<Ad>();
            id.Should().Be(ad.Id);
        }

        [Fact]
        public async Task OnlyUpdatesSpecifiedFields()
        {
            var input = new UpdateAd.Input
            {
                AdId = ad.GetIdentifier(),
                Category = AdCategory.StorageSpace
            };
            
            await handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var ad = await db.Ads.Include(x => x.Address).FirstAsync();
                ad.Category.Should().Be(AdCategory.StorageSpace);
                ad.Address.Latitude.Should().Be(46.8214098);
                ad.Address.Longitude.Should().Be(-71.237595);
                ad.Address.Locality.Should().Be("Québec");
                ad.Address.Neighborhood.Should().Be("Saint-Roch");
                ad.Address.Sublocality.Should().Be("La Cité-Limoilou");
                ad.Address.PostalCode.Should().Be("G1K0H1");
                ad.Address.Raw.Should().Be("{\"street_number\":\"395\",\"route\":\"Rue Bickell\",\"locality\":\"Québec\",\"administrative_area_level_2\":\"Communauté - Urbaine - de - Québec\",\"administrative_area_level_1\":\"QC\",\"country\":\"Canada\",\"latitude\":46.8214098,\"longitude\":-71.237595,\"neighborhood\":\"Saint - Roch\",\"sublocality\":\"La Cité - Limoilou\"}");
                ad.Address.Route.Should().Be("Rue Bickell");
                ad.Address.StreetNumber.Should().Be("395");
            }
        }

        [Fact]
        public async Task ShouldResetAddressOnAddressChange()
        {
            var input = new UpdateAd.Input
            {
                AdId = ad.GetIdentifier(),
                Category = AdCategory.StorageSpace,
                Address = new UpdateAd.AddressInput()
                {
                    Latitude = 45.5576387,
                    Longitude = -73.5515693,
                    Raw = "{\"street_number\":\"5650\",\"route\":\"Rue D'Iberville\",\"locality\":\"Montréal\",\"administrative_area_level_2\":\"Communauté-Urbaine-de-Montréal\",\"administrative_area_level_1\":\"QC\",\"country\":\"Canada\",\"postal_code\":\"H2G 2B3\",\"latitude\":45.544332,\"longitude\":-73.58167399999999}".NonNull(),
                }
            };

            await handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var ad = await db.Ads.Include(x => x.Address).FirstAsync();
                ad.Address.Latitude.Should().Be(45.5576387);
                ad.Address.Longitude.Should().Be(-73.5515693);
                ad.Address.Raw.Should().Be("{\"street_number\":\"5650\",\"route\":\"Rue D'Iberville\",\"locality\":\"Montréal\",\"administrative_area_level_2\":\"Communauté-Urbaine-de-Montréal\",\"administrative_area_level_1\":\"QC\",\"country\":\"Canada\",\"postal_code\":\"H2G 2B3\",\"latitude\":45.544332,\"longitude\":-73.58167399999999}");

                ad.Address.Locality.Should().Be("");
                ad.Address.Neighborhood.Should().Be("");
                ad.Address.Sublocality.Should().Be("");
                ad.Address.PostalCode.Should().Be("");
                ad.Address.Route.Should().Be("");
                ad.Address.StreetNumber.Should().Be("");
            }
        }

        [Fact]
        public async Task ShouldThrowErrorOnInvalidId()
        {
            var input = new UpdateAd.Input
            {
                AdId = "QWQ6MB=="
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateAd.AdNotFoundException>();
        }

        [Fact]
        public async Task ValidatesExistenceOfPictures()
        {
            var input = GetValidInput();
            input.GalleryItems = (new List<UpdateAd.GalleryItemInput>() { new UpdateAd.GalleryItemInput { Src = "test.png", Alt = "Texte" } }).NonNull();

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateAd.ImageNotFoundException>();

            fileManager.Setup(x => x.Exists(FileContainers.Images, "test.png"))
                .ReturnsAsync(true);

            var result = handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var ad = await db.Ads.Include(x => x.Gallery).FirstAsync();
                ad.Gallery.First().PictureFileId.Should().Be("test.png");
                ad.Gallery.Count.Should().Be(1);
            }
        }
    }
}
