using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Requests.Commands.Mutations.Accounts;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Accounts
{
    public class SendPasswordResetTests : TestBase
    {
        private readonly AppUser user;
        private readonly Mock<IMailer> mailer;
        private readonly IRequestHandler<SendPasswordReset.Input> handler;

        public SendPasswordResetTests()
        {
            user = AddUser("test@example.com", UserType.User);

            mailer = new Mock<IMailer>();
            handler = new SendPasswordReset(UserManager, mailer.Object, NullLogger<SendPasswordReset>.Instance);
        }
        
        [Fact]
        public async Task SendsResetEmailWithValidToken()
        {
            var input = new SendPasswordReset.Input {
                Email = "test@example.com"
            };

            await handler.Handle(input, CancellationToken.None);

            mailer.Verify(x => x.Send(It.Is<ResetPasswordEmail>(email => email.To.Contains("test@example.com"))));

            var token = mailer.Invocations[0].Arguments[0].As<ResetPasswordEmail>().Token;
            (await UserManager.ResetPasswordAsync(user, token, "1234aAuuuu")).AssertSuccess();
        }

        [Fact]
        public async Task DoesNothingIfUnknownEmail()
        {
            var input = new SendPasswordReset.Input {
                Email = "unknown@example.com"
            };

            await handler.Handle(input, CancellationToken.None);

            mailer.Verify(x => x.Send(It.IsAny<EmailModel>()), Times.Never);
        }
    }
}