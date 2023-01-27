using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Requests.Commands.Mutations.Ads;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Ads
{
    public class UnpublishAdTests : TestBase
    {
        private readonly UnpublishAd handler;
        private readonly AppUser user;
        private readonly Ad ad;

        public UnpublishAdTests()
        {
            handler = new UnpublishAd(DbContext, NullLogger<UnpublishAd>.Instance);
            user = AddUser("test@example.com", UserType.User);
            SetLoggedInUser(user);

            ad = new Ad()
            {
                Category = AdCategory.DeliveryTruck,
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
                IsPublish = true
            };

            DbContext.Ads.Add(ad);
            DbContext.SaveChanges();
        }

        private UnpublishAd.Input GetValidInput()
        {
            return new UnpublishAd.Input
            {
                AdId = ad.GetIdentifier()
            };
        }

        [Fact]
        public async Task ShouldUnpublishAd()
        {
            var input = GetValidInput();
            await handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var ad = await db.Ads.FirstAsync();

                ad.IsPublish.Should().Be(false);
            }
        }

        [Fact]
        public async Task ShouldThrowErrorOnInvalidId()
        {
            var input = new UnpublishAd.Input
            {
                AdId = "QWQ6MB=="
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UnpublishAd.AdNotFoundException>();
        }
    }
}
