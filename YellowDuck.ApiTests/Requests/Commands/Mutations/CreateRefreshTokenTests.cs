using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Requests.Commands;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations
{
    public class CreateRefreshTokenTests : TestBase
    {
        private readonly CreateRefreshToken handler;

        public CreateRefreshTokenTests()
        {
            handler = new CreateRefreshToken(UserManager);
        }

        [Fact]
        public async Task ThrowsIfUserNotFound()
        {
            var input = new CreateRefreshToken.Command("asdf", "deviceId");

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<CreateRefreshToken.UserNotFoundException>();
        }

        [Fact]
        public async Task ReturnAValidAccessToken()
        {
            var user = AddUser("test@example.com", UserType.User);

            var input = new CreateRefreshToken.Command(user.Id, "deviceId");

            var refreshToken = await handler.Handle(input, CancellationToken.None);

            refreshToken.Should().NotBeNullOrEmpty();
        }
    }
}
