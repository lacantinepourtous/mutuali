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
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Requests.Commands.Mutations.Ads;
using YellowDuck.Api.Requests.Commands.Mutations.Contracts;
using YellowDuck.Api.Requests.Commands.Mutations.Conversations;
using YellowDuck.Api.Services.Files;
using YellowDuck.Api.Services.Twilio.Conversations;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Contracts
{
    public class UpdateContractTests : TestBase
    {
        private readonly Mock<IFileManager> fileManager;
        private readonly Mock<IConversationsService> conversationsService;

        private readonly AppUser user1;
        private readonly AppUser user2;
        
        private Contract contract;
        
        private readonly CreateAd createAdHandler;
        private readonly CreateConversation createConversationHandler;
        private readonly UpdateContract handler;

        public UpdateContractTests()
        {
            fileManager = new Mock<IFileManager>();
            conversationsService = new Mock<IConversationsService>();

            handler = new UpdateContract(DbContext, NullLogger<UpdateContract>.Instance, fileManager.Object, UserManager, UserAccessor, Mediator);
            createAdHandler = new CreateAd(DbContext, UserManager, NullLogger<CreateAd>.Instance, fileManager.Object, UserAccessor);
            createConversationHandler = new CreateConversation(DbContext, NullLogger<CreateConversation>.Instance, conversationsService.Object, UserAccessor, UserManager);

            user1 = AddUser("test1@example.com", UserType.User);
            user2 = AddUser("test2@example.com", UserType.User);
        }

        private async Task SetupContract(AppUser user1, AppUser user2)
        {
            SetLoggedInUser(user2);
            var inputAd = new CreateAd.Input
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
            var resultAd = await createAdHandler.Handle(inputAd, CancellationToken.None);
            var adId = resultAd.Ad.Id;

            conversationsService.Setup(x => x.Create(new AppUser[2] { user2, user1 }))
                .ReturnsAsync(new Conversation() { AdId = adId.LongIdentifierForType<Ad>(), Sid = "CH0756f4faae444273b518ef0907622b02", Participants = new List<ConversationParticipant>() { new ConversationParticipant() { Sid = "MBfc0410e7ad8c41948d6be48b37bbcc57", UserId = user1.Id }, new ConversationParticipant() { Sid = "MBe24fba520f06430b99a36a780b23991d", UserId = user2.Id }, new ConversationParticipant() { Sid = "MB826e1fdc2f77498e88b263a144fa81f6" } } });

            SetLoggedInUser(user1);

            var inputConversation = new CreateConversation.Input
            {
                AdId = adId
            };

            var resultConversation = await createConversationHandler.Handle(inputConversation, CancellationToken.None);
            SetLoggedInUser(user2);

            var conversation = DbContext.Conversations.Where(x => resultConversation.Conversation.Id.LongIdentifierForType<Conversation>() == x.Id).First();
            contract = new Contract()
            {
                OwnerId = user2.Id,
                TenantId = user1.Id,
                DatePrecision = "Précision à propos de la date",
                StartDate = new DateTime(2021, 10, 10),
                EndDate = new DateTime(2022, 7, 22),
                Price = 1000,
                Terms = "Ceci est un texte de test pour les termes et conditions",
                ConversationId = conversation.Id
            };

            conversation.Contract = contract;

            DbContext.Contracts.Add(contract);
            DbContext.SaveChanges();
        }

        private UpdateContract.Input GetValidInput()
        {
            var input = new UpdateContract.Input
            {
                ContractId = contract.GetIdentifier(),
                DatePrecision = "Nouvelle précision à propos de la date",
                StartDate = new DateTime(2022, 10, 10),
                EndDate = new DateTime(2023, 7, 22),
                Price = 100,
                Terms = "Ceci est un nouveau texte de test pour les termes et conditions"
            };

            return input;
        }

        [Fact]
        public async Task ShouldReturnTheUpdatedContract()
        {
            await SetupContract(user1, user2);
            var input = GetValidInput();
            var result = await handler.Handle(input, CancellationToken.None);

            result.Contract.Should().NotBeNull();

            var price = await result.Contract.Price;
            price.Should().Be(100);

            var id = result.Contract.Id.LongIdentifierForType<Contract>();
            id.Should().Be(contract.Id);
        }

        [Fact]
        public async Task ShouldThrowErrorOnInvalidId()
        {
            await SetupContract(user1, user2);
            var input = new UpdateContract.Input
            {
                ContractId = "Q29udHJhY3Q6MB=="
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateContract.ContractNotFound>();
        }

        [Fact]
        public async Task OnlyUpdatesSpecifiedFields()
        {
            await SetupContract(user1, user2);
            var input = new UpdateContract.Input
            {
                ContractId = contract.GetIdentifier(),
                Price = 100
            };

            await handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var contract = await db.Contracts.FirstAsync();
                contract.Price.Should().Be(100);
                contract.DatePrecision.Should().Be("Précision à propos de la date");
                contract.StartDate.Should().Be(new DateTime(2021, 10, 10));
                contract.EndDate.Should().Be(new DateTime(2022, 7, 22));
                contract.Terms.Should().Be("Ceci est un texte de test pour les termes et conditions");
            }
        }

        [Fact]
        public async Task ValidatesExistenceOfFiles()
        {
            await SetupContract(user1, user2);
            var input = GetValidInput();
            input.FileItems = (new List<string>() { "test.pdf" }).NonNull();

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<UpdateContract.FileNotFoundException>();

            fileManager.Setup(x => x.Exists(FileContainers.Files, "test.pdf"))
                .ReturnsAsync(true);

            var result = handler.Handle(input, CancellationToken.None);

            using (var db = CreateDbContext())
            {
                var contract = await db.Contracts.Include(x => x.Files).FirstAsync();
                contract.Files.First().FileId.Should().Be("test.pdf");
                contract.Files.Count.Should().Be(1);
            }
        }
    }
}
