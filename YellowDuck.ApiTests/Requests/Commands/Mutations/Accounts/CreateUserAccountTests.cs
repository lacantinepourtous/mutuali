using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Requests.Commands.Mutations.Accounts;
using YellowDuck.Api.Services.Mailer;
using YellowDuck.Api.Services.Phone;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static YellowDuck.Api.Requests.Commands.Mutations.Accounts.CreateUserAccount;
using YellowDuck.Api.DbModel.Entities;
using System;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Accounts
{
    public class CreateUserAccountTest : TestBase
    {
        private readonly CreateUserAccount handler;
        private readonly Mock<IMailer> mailer;
        private readonly Mock<IPhoneVerificationService> phoneVerificationService;

        private const string Email = "test@example.com";
        private const string Password = "1234aAuuuu";
        private const string FirstName = "Example";
        private const string LastName = "Bliblou";
        private const string OrganizationName = "Example Inc.";
        private const string OrganizationNEQ = "0000000000";
        private const string PostalCode = "G1K 0H1";
        private const string PhoneNumber = "5145551234";
        private const bool ShowPhoneNumber = true;
        private const bool ShowEmail = false;


        public CreateUserAccountTest()
        {
            mailer = new Mock<IMailer>();
            phoneVerificationService = new Mock<IPhoneVerificationService>();
            handler = new CreateUserAccount(
                UserManager,
                DbContext,
                mailer.Object,
                NullLogger<CreateUserAccount>.Instance,
                HttpContextAccessor,
                phoneVerificationService.Object);
        }

        [Fact]
        public async Task CreatesTheAccount()
        {
            DbContext.PhoneVerifications.Add(new PhoneVerification() { PhoneNumber = PhoneNumber, IsVerified = true });
            await DbContext.SaveChangesAsync();
            var input = new Input
            {
                Email = Email,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                OrganizationName = OrganizationName,
                OrganizationNEQ = OrganizationNEQ,
                OrganizationType = OrganizationType.NonProfit,
                Industry = Industry.HealthAndSocialServices,
                PhoneNumber = PhoneNumber,
                ShowPhoneNumber = ShowPhoneNumber,
                ShowEmail = ShowEmail
            };

            await handler.Handle(input, CancellationToken.None);

            var user = await DbContext.Users.Include(x => x.Profile).FirstAsync();

            user.Email.Should().Be(Email);
            user.Type.Should().Be(UserType.User);
            user.EmailConfirmed.Should().BeFalse();
            user.AcceptedTos.Should().Be(TosVersion.Latest);
            user.Profile.FirstName.Should().Be(FirstName);
            user.Profile.LastName.Should().Be(LastName);
            user.Profile.OrganizationName.Should().Be(OrganizationName);
            user.Profile.OrganizationNEQ.Should().Be(OrganizationNEQ);
            user.Profile.OrganizationType.Should().Be(OrganizationType.NonProfit);
            user.Profile.Industry.Should().Be(Industry.HealthAndSocialServices);
            user.Profile.PhoneNumber.Should().Be(PhoneNumber);
            user.Profile.ShowPhoneNumber.Should().Be(ShowPhoneNumber);
            user.Profile.ShowEmail.Should().Be(ShowEmail);
        }

        [Fact]
        public async Task SendsConfirmationEmail()
        {
            DbContext.PhoneVerifications.Add(new PhoneVerification() { PhoneNumber = PhoneNumber, IsVerified = true });
            await DbContext.SaveChangesAsync();
            var input = new Input
            {
                Email = Email,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                OrganizationName = OrganizationName,
                OrganizationNEQ = OrganizationNEQ,
                OrganizationType = OrganizationType.NonProfit,
                Industry = Industry.HealthAndSocialServices,
                PhoneNumber = PhoneNumber,
                ShowPhoneNumber = ShowPhoneNumber,
                ShowEmail = ShowEmail
            };

            await handler.Handle(input, CancellationToken.None);

            mailer.Verify(x => x.Send(It.IsAny<ConfirmEmailEmail>()));
        }

        [Fact]
        public async Task ThrowsWhenPhoneNumberNotVerified()
        {
            var input = new Input
            {
                Email = Email,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                OrganizationName = OrganizationName,
                OrganizationNEQ = OrganizationNEQ,
                OrganizationType = OrganizationType.NonProfit,
                Industry = Industry.HealthAndSocialServices,
                PhoneNumber = PhoneNumber,
                ShowPhoneNumber = ShowPhoneNumber,
                ShowEmail = ShowEmail
            };

            await handler.Invoking(x => x.Handle(input, CancellationToken.None))
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Phone number is not verified.");
        }
    }
}
