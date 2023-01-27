using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Requests.Commands.Mutations.Accounts;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Accounts
{
    public class AcceptTosTests : TestBase
    {
        private readonly AcceptTos handler;
        private readonly AppUser user;

        public AcceptTosTests()
        {
            handler = new AcceptTos(DbContext, UserAccessor, HttpContextAccessor);
            user = AddUser("test@example.com", UserType.User);
            SetLoggedInUser(user);
        }

        [Fact]
        public async Task CanAcceptTos()
        {
            user.AcceptedTos = TosVersion.Undefined;
            var input = new AcceptTos.Input { TosVersion = TosVersion.Version1 };
            await handler.Handle(input, CancellationToken.None);

            user.AcceptedTos.Should().Be(TosVersion.Version1);
        }

        [Fact]
        public async Task CantAcceptTosIfAlreadyAccepted()
        {
            user.AcceptedTos = TosVersion.Version1;

            var input = new AcceptTos.Input { TosVersion = TosVersion.Version1 };
            var exception = await Record.ExceptionAsync(() => handler.Handle(input, CancellationToken.None));

            exception.Should().BeOfType<AcceptTos.VersionAlreadyAcceptedException>();
        }
    }
}
