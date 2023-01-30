using FluentAssertions;
using GraphQL.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Requests.Commands.Mutations.Ads;
using YellowDuck.Api.Requests.Commands.Mutations.Conversations;
using YellowDuck.Api.Services.Files;
using YellowDuck.Api.Services.Twilio.Conversations;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Communications
{
    public class CreateConversationTests : TestBase
    {
        private readonly Mock<IFileManager> fileManager;
        private readonly Mock<IConversationsService> conversationsService;
        private readonly CreateAd createAdHandler;
        private readonly AppUser user1;
        private readonly AppUser user2;
        private Id adId;

        public CreateConversationTests()
        {
            fileManager = new Mock<IFileManager>();
            createAdHandler = new CreateAd(DbContext, UserManager, NullLogger<CreateAd>.Instance, fileManager.Object, UserAccessor);

            conversationsService = new Mock<IConversationsService>();
            user1 = AddUser("test1@example.com", UserType.User);
            user2 = AddUser("test2@example.com", UserType.User);
        }

        private async Task SetupAd(AppUser user1, AppUser user2)
        {
            SetLoggedInUser(user2);
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
                }.NonNull()
            };
            var result = await createAdHandler.Handle(input, CancellationToken.None);
            adId = result.Ad.Id;

            conversationsService.Setup(x => x.Create(new AppUser[2] { user2, user1 }))
                .ReturnsAsync(new Conversation() { AdId = adId.LongIdentifierForType<Ad>(), Sid = "CH0756f4faae444273b518ef0907622b02", Participants = new List<ConversationParticipant>() { new ConversationParticipant() { Sid = "MBfc0410e7ad8c41948d6be48b37bbcc57", UserId = user1.Id }, new ConversationParticipant() { Sid = "MBe24fba520f06430b99a36a780b23991d", UserId = user2.Id }, new ConversationParticipant() { Sid = "MB826e1fdc2f77498e88b263a144fa81f6" } } });

            SetLoggedInUser(user1);
        }

        [Fact]
        public async Task ShouldAddTheConversationToDb()
        {
            await SetupAd(user1, user2);
            var handler = new CreateConversation(DbContext, NullLogger<CreateConversation>.Instance, conversationsService.Object, UserAccessor, UserManager);

            var input = new CreateConversation.Input
            {
                AdId = adId
            };

            await handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var conversation = await db.Conversations.Include(x => x.Participants).FirstAsync();

                conversation.Sid.Should().Be("CH0756f4faae444273b518ef0907622b02");

                conversation.Participants.Count.Should().Be(3);

                conversation.Participants[0].Sid.Should().Be("MBe24fba520f06430b99a36a780b23991d");
                conversation.Participants[0].UserId.Should().Be(user2.Id);

                conversation.Participants[1].Sid.Should().Be("MBfc0410e7ad8c41948d6be48b37bbcc57");
                conversation.Participants[1].UserId.Should().Be(user1.Id);

                conversation.Participants[2].Sid.Should().Be("MB826e1fdc2f77498e88b263a144fa81f6");
                conversation.Participants[2].UserId.Should().Be(null);
            }
        }

        [Fact]
        public async Task ShouldReturnTheCreatedConversation()
        {
            await SetupAd(user1, user2);
            var handler = new CreateConversation(DbContext, NullLogger<CreateConversation>.Instance, conversationsService.Object, UserAccessor, UserManager);

            var input = new CreateConversation.Input
            {
                AdId = adId
            };

            var result = await handler.Handle(input, CancellationToken.None);

            var category = await result.Conversation.Sid;
            category.Should().Be("CH0756f4faae444273b518ef0907622b02");
        }

        [Fact]
        public async Task CantStartConversationWithSelf()
        {
            await SetupAd(user1, user1);
            var handler = new CreateConversation(DbContext, NullLogger<CreateConversation>.Instance, conversationsService.Object, UserAccessor, UserManager);

            var input = new CreateConversation.Input
            {
                AdId = adId
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<CreateConversation.CantStartConversationWithSelf>();
        }

        [Fact]
        public async Task ShouldThrowErrorOnInvalidAdId()
        {
            await SetupAd(user1, user2);
            var handler = new CreateConversation(DbContext, NullLogger<CreateConversation>.Instance, conversationsService.Object, UserAccessor, UserManager);

            var input = new CreateConversation.Input
            {
                AdId = "QWQ6MB=="
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<CreateConversation.AdNotFoundException>();
        }
    }
}
