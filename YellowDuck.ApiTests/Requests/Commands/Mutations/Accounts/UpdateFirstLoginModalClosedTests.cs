using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Requests.Commands.Mutations.Accounts;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Accounts
{
    public class UpdateFirstLoginModalClosedTests : TestBase
    {
        private readonly UpdateFirstLoginModalClosed handler;
        private readonly AppUser user;

        public UpdateFirstLoginModalClosedTests()
        {
            handler = new UpdateFirstLoginModalClosed(DbContext, UserAccessor);
            user = AddUser("test@example.com", UserType.User);
            SetLoggedInUser(user);
        }

        [Fact]
        public async Task CanUpdateFirstLoginModalClosed()
        {
            user.FirstLoginModalClosed = false;
            var input = new UpdateFirstLoginModalClosed.Input { FirstLoginModalClosed = true };
            await handler.Handle(input, CancellationToken.None);

            user.FirstLoginModalClosed.Should().Be(true);
        }
    }
}
