using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
using YellowDuck.Api.Requests.Queries.Ads;
using YellowDuck.Api.Services.System;

namespace YellowDuck.ApiTests.Requests.Queries.Ads
{
  public class SearchAdsTests : TestBase
  {
    private readonly SearchAds handler;
    private readonly AppUser user1;
    private readonly AppUser user2;
    private readonly AppUser admin;
    private readonly Ad publishedAd;
    private readonly Ad unpublishedAd;
    private readonly Ad user1UnpublishedAd;
    private readonly Ad adminOnlyAd;

    public SearchAdsTests()
    {
      handler = new SearchAds(DbContext);

      // Créer les utilisateurs
      user1 = AddUser("user1@example.com", UserType.User);
      user2 = AddUser("user2@example.com", UserType.User);
      admin = AddUser("admin@example.com", UserType.Admin);

      // Créer les annonces
      publishedAd = CreateAd(user1, "Published Ad", true, false);
      unpublishedAd = CreateAd(user2, "Unpublished Ad", false, false);
      user1UnpublishedAd = CreateAd(user1, "User1 Unpublished Ad", false, false);
      adminOnlyAd = CreateAd(user1, "Admin Only Ad", true, true);

      DbContext.SaveChanges();
    }

    private Ad CreateAd(AppUser user, string title, bool isPublish, bool isAdminOnly)
    {
      var ad = new Ad
      {
        UserId = user.Id,
        User = user,
        Category = AdCategory.StorageSpace,
        IsPublish = isPublish,
        IsAdminOnly = isAdminOnly,
        Translations = new List<AdTranslation>
                {
                    new AdTranslation
                    {
                        Title = title,
                        Description = "Description",
                        Language = ContentLanguage.French
                    }
                }
      };

      DbContext.Ads.Add(ad);
      return ad;
    }

    [Fact]
    public async Task WhenUserNotLoggedIn_ShouldReturnOnlyPublishedAds()
    {
      // Arrange
      var query = new SearchAds.Query();

      // Act
      var result = await handler.Handle(query, CancellationToken.None);

      // Assert
      result.Should().HaveCount(1);
      result.First().IsPublish.Should().BeTrue();
      result.First().IsAdminOnly.Should().BeFalse();
    }

    [Fact]
    public async Task WhenUserLoggedIn_ShouldReturnPublishedAdsAndOwnUnpublishedAds()
    {
      // Arrange
      SetLoggedInUser(user1);
      var query = new SearchAds.Query
      {
        CurrentUserId = user1.Id,
        IsAdmin = false
      };

      // Act
      var result = await handler.Handle(query, CancellationToken.None);

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain(x => x.Id == publishedAd.Id);
      result.Should().Contain(x => x.Id == user1UnpublishedAd.Id);
      result.Should().NotContain(x => x.Id == unpublishedAd.Id); // Pas l'annonce non publiée d'un autre utilisateur
    }

    [Fact]
    public async Task WhenAdmin_ShouldReturnAllAds()
    {
      // Arrange
      SetLoggedInUser(admin);
      var query = new SearchAds.Query
      {
        CurrentUserId = admin.Id,
        IsAdmin = true
      };

      // Act
      var result = await handler.Handle(query, CancellationToken.None);

      // Assert
      result.Should().HaveCount(4); // Toutes les annonces
    }

    [Fact]
    public async Task WhenAdminWithShowAdminOnly_ShouldReturnAllAdsIncludingAdminOnly()
    {
      // Arrange
      SetLoggedInUser(admin);
      var query = new SearchAds.Query
      {
        CurrentUserId = admin.Id,
        IsAdmin = true,
        ShowAdminOnly = true
      };

      // Act
      var result = await handler.Handle(query, CancellationToken.None);

      // Assert
      result.Should().HaveCount(4);
      result.Should().Contain(x => x.Id == adminOnlyAd.Id);
    }

    [Fact]
    public async Task WhenUserLoggedIn_ShouldNotReturnAdminOnlyAds()
    {
      // Arrange
      SetLoggedInUser(user1);
      var query = new SearchAds.Query
      {
        CurrentUserId = user1.Id,
        IsAdmin = false
      };

      // Act
      var result = await handler.Handle(query, CancellationToken.None);

      // Assert
      result.Should().NotContain(x => x.Id == adminOnlyAd.Id);
    }

    [Fact]
    public async Task WhenFilteringByCategory_ShouldApplyFilter()
    {
      // Arrange
      var query = new SearchAds.Query
      {
        Category = AdCategory.StorageSpace
      };

      // Act
      var result = await handler.Handle(query, CancellationToken.None);

      // Assert
      result.Should().HaveCount(1);
      result.First().Category.Should().Be(AdCategory.StorageSpace);
    }

    [Fact]
    public async Task WhenFilteringByDeliveryTruckType_ShouldApplyFilter()
    {
      // Arrange
      publishedAd.DeliveryTruckType = DeliveryTruckType.Van;
      DbContext.SaveChanges();

      var query = new SearchAds.Query
      {
        DeliveryTruckType = DeliveryTruckType.Van
      };

      // Act
      var result = await handler.Handle(query, CancellationToken.None);

      // Assert
      result.Should().HaveCount(1);
      result.First().DeliveryTruckType.Should().Be(DeliveryTruckType.Van);
    }

    [Fact]
    public async Task WhenFilteringByRefrigerated_ShouldApplyFilter()
    {
      // Arrange
      publishedAd.Refrigerated = true;
      DbContext.SaveChanges();

      var query = new SearchAds.Query
      {
        Refrigerated = true
      };

      // Act
      var result = await handler.Handle(query, CancellationToken.None);

      // Assert
      result.Should().HaveCount(1);
      result.First().Refrigerated.Should().BeTrue();
    }
  }
}