using FluentAssertions;
using GraphQL.Conventions;
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
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Requests.Commands.Mutations.Ads;
using YellowDuck.Api.Requests.Commands.Mutations.Contracts;
using YellowDuck.Api.Requests.Commands.Mutations.Conversations;
using YellowDuck.Api.Services.Files;
using YellowDuck.Api.Services.Twilio.Conversations;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Contracts
{
    public class CreateContractTests : TestBase
    {
        private readonly Mock<IFileManager> fileManager;
        private readonly Mock<IConversationsService> conversationsService;
        private readonly CreateAd createAdHandler;
        private readonly CreateConversation createConversationHandler;
        private readonly AppUser user1;
        private readonly AppUser user2;
        private Id adId;
        private Id conversationId;

        public CreateContractTests()
        {
            fileManager = new Mock<IFileManager>();
            conversationsService = new Mock<IConversationsService>();

            createAdHandler = new CreateAd(DbContext, UserManager, NullLogger<CreateAd>.Instance, fileManager.Object, UserAccessor);
            createConversationHandler = new CreateConversation(DbContext, NullLogger<CreateConversation>.Instance, conversationsService.Object, UserAccessor, UserManager);

            user1 = AddUser("test1@example.com", UserType.User);
            user2 = AddUser("test2@example.com", UserType.User);
        }

        private async Task SetupConversation(AppUser user1, AppUser user2)
        {
            SetLoggedInUser(user2);
            var inputAd = new CreateAd.Input
            {
                Title = "Test ad",
                Category = AdCategory.DeliveryTruck,
                DeliveryTruckType = DeliveryTruckType.Van,
                Description = new Maybe<NonNull<string>>("Test ad description"),
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
            var resultAd = await createAdHandler.Handle(inputAd, CancellationToken.None);
            adId = resultAd.Ad.Id;

            conversationsService.Setup(x => x.Create(new AppUser[2] { user2, user1 }))
                .ReturnsAsync(new Conversation() { AdId = adId.LongIdentifierForType<Ad>(), Sid = "CH0756f4faae444273b518ef0907622b02", Participants = new List<ConversationParticipant>() { new ConversationParticipant() { Sid = "MBfc0410e7ad8c41948d6be48b37bbcc57", UserId = user1.Id }, new ConversationParticipant() { Sid = "MBe24fba520f06430b99a36a780b23991d", UserId = user2.Id }, new ConversationParticipant() { Sid = "MB826e1fdc2f77498e88b263a144fa81f6" } } });

            SetLoggedInUser(user1);

            var inputConversation = new CreateConversation.Input
            {
                AdId = adId
            };

            var resultConversation = await createConversationHandler.Handle(inputConversation, CancellationToken.None);
            conversationId = resultConversation.Conversation.Id;

            SetLoggedInUser(user2);
        }

        [Fact]
        public async Task ShouldAddTheContractToDb()
        {
            await SetupConversation(user1, user2);
            var handler = new CreateContract(DbContext, NullLogger<CreateContract>.Instance, fileManager.Object, UserManager, UserAccessor, Mediator);

            var input = new CreateContract.Input
            {
                ConversationId = conversationId,
                DatePrecision = "Précision à propos de la date",
                StartDate = new DateTime(2021, 10, 10),
                EndDate = new DateTime(2022, 7, 22),
                Price = 1000,
                Terms = "Ceci est un texte de test pour les termes et conditions",
                FileItems = (new List<string>() { "test.pdf" }).NonNull()
            };

            fileManager.Setup(x => x.Exists(FileContainers.Files, "test.pdf"))
                .ReturnsAsync(true);

            var result = await handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var contract = await db.Contracts
                    .Include(x => x.Files)
                    .FirstAsync();
                contract.ConversationId.Should().Be(conversationId.LongIdentifierForType<Conversation>());
                contract.DatePrecision.Should().Be("Précision à propos de la date");
                contract.StartDate.Should().Be(new DateTime(2021, 10, 10));
                contract.EndDate.Should().Be(new DateTime(2022, 7, 22));
                contract.Price.Should().Be(1000);
                contract.Terms.Should().Be("Ceci est un texte de test pour les termes et conditions");
                contract.Files.First().FileId.Should().Be("test.pdf");
            }
        }

        [Fact]
        public async Task ShouldReturnTheCreatedContract()
        {
            await SetupConversation(user1, user2);
            var handler = new CreateContract(DbContext, NullLogger<CreateContract>.Instance, fileManager.Object, UserManager, UserAccessor, Mediator);

            var input = new CreateContract.Input
            {
                ConversationId = conversationId,
                DatePrecision = "Précision à propos de la date",
                StartDate = new DateTime(2021, 10, 10),
                EndDate = new DateTime(2022, 7, 22),
                Price = 1000,
                Terms = "Ceci est un texte de test pour les termes et conditions"
            };

            var result = await handler.Handle(input, CancellationToken.None);

            var price = await result.Contract.Price;
            price.Should().Be(1000);
        }

        [Fact]
        public async Task ValidateConversationNotFound()
        {
            await SetupConversation(user1, user2);
            var handler = new CreateContract(DbContext, NullLogger<CreateContract>.Instance, fileManager.Object, UserManager, UserAccessor, Mediator);

            var input = new CreateContract.Input
            {
                ConversationId = Id.New<Conversation>(98709384908),
                DatePrecision = "Précision à propos de la date",
                StartDate = new DateTime(2021, 10, 10),
                EndDate = new DateTime(2022, 7, 22),
                Price = 1000,
                Terms = "Ceci est un texte de test pour les termes et conditions"
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<CreateContract.ConversationNotFound>();
        }

        [Fact]
        public async Task ValidateUserNotOwner()
        {
            await SetupConversation(user1, user2);
            SetLoggedInUser(user1);
            var handler = new CreateContract(DbContext, NullLogger<CreateContract>.Instance, fileManager.Object, UserManager, UserAccessor, Mediator);

            var input = new CreateContract.Input
            {
                ConversationId = conversationId,
                DatePrecision = "Précision à propos de la date",
                StartDate = new DateTime(2021, 10, 10),
                EndDate = new DateTime(2022, 7, 22),
                Price = 1000,
                Terms = "Ceci est un texte de test pour les termes et conditions"
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<CreateContract.UserNotOwner>();
        }

        [Fact]
        public async Task ValidatesExistenceOfFiles()
        {
            await SetupConversation(user1, user2);
            var handler = new CreateContract(DbContext, NullLogger<CreateContract>.Instance, fileManager.Object, UserManager, UserAccessor, Mediator);

            var input = new CreateContract.Input
            {
                ConversationId = conversationId,
                DatePrecision = "Précision à propos de la date",
                StartDate = new DateTime(2021, 10, 10),
                EndDate = new DateTime(2022, 7, 22),
                Price = 1000,
                Terms = "Ceci est un texte de test pour les termes et conditions",
                FileItems = (new List<string>() { "test.pdf" }).NonNull()
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<CreateContract.FileNotFoundException>();

            fileManager.Setup(x => x.Exists(FileContainers.Files, "test.pdf"))
                .ReturnsAsync(true);

            var result = await handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var contract = await db.Contracts
                    .Include(x => x.Files)
                    .FirstAsync();
                contract.ConversationId.Should().Be(conversationId.LongIdentifierForType<Conversation>());
                contract.Files.First().FileId.Should().Be("test.pdf");
            }
        }
    }
}
