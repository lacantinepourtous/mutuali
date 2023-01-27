using FluentAssertions;
using GraphQL.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Requests.Commands.Mutations.Ads;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Ads
{
    public class UpdateAdTranslationTests : TestBase
    {
        private readonly UpdateAdTranslation handler;
        private readonly AppUser user;
        private readonly Ad ad;

        public UpdateAdTranslationTests()
        {
            handler = new UpdateAdTranslation(DbContext, NullLogger<UpdateAdTranslation>.Instance);
            user = AddUser("test@example.com", UserType.User);
            SetLoggedInUser(user);

            ad = new Ad()
            {
                Category = AdCategory.DeliveryTruck,
                Address = new AdAddress()
                {
                    Latitude = 46.8214098,
                    Longitude = -71.237595,
                    Raw = "{\"street_number\":\"395\",\"route\":\"Rue Bickell\",\"locality\":\"Québec\",\"administrative_area_level_2\":\"Communauté - Urbaine - de - Québec\",\"administrative_area_level_1\":\"QC\",\"country\":\"Canada\",\"latitude\":46.8214098,\"longitude\":-71.237595,\"neighborhood\":\"Saint - Roch\",\"sublocality\":\"La Cité - Limoilou\"}"
                },
                Translations = new List<AdTranslation>()
                {
                    new AdTranslation()
                    {
                        Title = "Titre test 1",
                        Description = "<h3>Ceci est une description de test</h3>",
                        Language = ContentLanguage.French,
                        SurfaceDescription = "<p>Test surface</p>",
                        DeliveryTruckTypeOther = "Moto"
                    }
                },
                Gallery = new List<AdGalleryItem>()
                {
                    new AdGalleryItem()
                    {
                        PictureFileId = "cbecdbac-33cf-4268-b0da-fbbbb11c17a5/favicon.PNG",
                        Alt = "Texte"
                    }
                }
            };

            DbContext.Ads.Add(ad);
            DbContext.SaveChanges();
        }

        private UpdateAdTranslation.Input GetValidInput()
        {
            var input = new UpdateAdTranslation.Input
            {
                AdId = ad.GetIdentifier(),
                Language = ContentLanguage.French,
                Title = "Ceci est un titre de test".NonNull(),
                Description = "<p>Ceci est un texte de test</p>".NonNull(),
                SurfaceDescription = new Maybe<NonNull<string>>("<p>Test surface updated</p>"),
                DeliveryTruckTypeOther = new Maybe<NonNull<string>>("Moto de livraison")
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
                var ad = await db.Ads.Include(x => x.Translations).FirstAsync();

                ad.Translations.First().Language.Should().Be(ContentLanguage.French);
                ad.Translations.First().Title.Should().Be("Ceci est un titre de test");
                ad.Translations.First().Description.Should().Be("<p>Ceci est un texte de test</p>");
                ad.Translations.First().SurfaceDescription.Should().Be("<p>Test surface updated</p>");
                ad.Translations.First().DeliveryTruckTypeOther.Should().Be("Moto de livraison");
            }
        }

        [Fact]
        public async Task ShouldReturnTheUpdatedAdTranslation()
        {
            var input = GetValidInput();
            var result = await handler.Handle(input, CancellationToken.None);

            result.AdTranslation.Should().NotBeNull();
            var title = await result.AdTranslation.Title;
            title.Should().Be("Ceci est un titre de test");
            var description = await result.AdTranslation.Description;
            description.Should().Be("<p>Ceci est un texte de test</p>");
        }

        [Fact]
        public async Task OnlyUpdatesSpecifiedFields()
        {
            var input = new UpdateAdTranslation.Input
            {
                AdId = ad.GetIdentifier(),
                Language = ContentLanguage.French,
                Description = "<p>Ceci est un texte de test</p>".NonNull()
            };

            var result = await handler.Handle(input, CancellationToken.None);

            var title = await result.AdTranslation.Title;
            title.Should().Be("Titre test 1");
            var description = await result.AdTranslation.Description;
            description.Should().Be("<p>Ceci est un texte de test</p>");
        }

        [Fact]
        public async Task ShouldThrowErrorOnInvalidId()
        {
            var input = new UpdateAdTranslation.Input
            {
                AdId = "QWQ6MB==",
                Language = ContentLanguage.French,
                Title = "Update title".NonNull()
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateAdTranslation.AdTranslationNotFoundException>();
        }

        [Fact]
        public async Task ShouldThrowErrorOnInvalidLanguage()
        {
            var input = new UpdateAdTranslation.Input
            {
                AdId = ad.GetIdentifier(),
                Language = ContentLanguage.English,
                Title = "Updated title".NonNull()
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateAdTranslation.AdTranslationNotFoundException>();
        }

        [Fact]
        public async Task ShouldThrowErrorIfNothingToUpdate()
        {
            var input = new UpdateAdTranslation.Input
            {
                AdId = ad.GetIdentifier(),
                Language = ContentLanguage.French
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateAdTranslation.NothingToUpdateException>();
        }

        [Fact]
        public async Task ShouldThrowErrorIfEmptyTitle()
        {
            var input = new UpdateAdTranslation.Input
            {
                AdId = ad.GetIdentifier(),
                Language = ContentLanguage.French,
                Title = "".NonNull()
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateAdTranslation.EmptyTitleException>();
        }

        [Fact]
        public async Task ShouldThrowErrorIfEmptyDescription()
        {
            var input = new UpdateAdTranslation.Input
            {
                AdId = ad.GetIdentifier(),
                Language = ContentLanguage.French,
                Description = "".NonNull()
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateAdTranslation.EmptyDescriptionException>();
        }
    }
}
