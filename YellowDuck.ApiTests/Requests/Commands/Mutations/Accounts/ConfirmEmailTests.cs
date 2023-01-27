using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Requests.Commands.Mutations.Accounts;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Accounts
{
    public class ConfirmEmailTests : TestBase
    {
        private readonly AppUser user;
        private readonly IRequestHandler<ConfirmEmail.Input> handler;

        public ConfirmEmailTests()
        {
            user = AddUser("test@example.com", UserType.User);
            user.EmailConfirmed = false;

            handler = new ConfirmEmail(UserManager, NullLogger<ConfirmEmail>.Instance);
        }

        [Fact]
        public async Task MarksEmailAsConfirmed()
        {
            var input = new ConfirmEmail.Input
            {
                Email = "test@example.com",
                Token = await UserManager.GenerateEmailConfirmationTokenAsync(user)
            };

            await handler.Handle(input, CancellationToken.None);

            user.EmailConfirmed.Should().BeTrue();
        }
    }
}
