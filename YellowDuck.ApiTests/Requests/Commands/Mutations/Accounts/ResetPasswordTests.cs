using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Requests.Commands.Mutations.Accounts;

namespace YellowDuck.ApiTests.Requests.Commands.Mutations.Accounts
{
    public class ResetPasswordTests : TestBase
    {
        private readonly AppUser user;
        private readonly ResetPassword handler;

        public ResetPasswordTests()
        {
            user = AddUser("test@example.com", UserType.User);

            handler = new ResetPassword(UserManager, NullLogger<ResetPassword>.Instance);
        }

        [Fact]
        public async Task ResetsThePassword()
        {
            var input = new ResetPassword.Input {
                EmailAddress = "test@example.com",
                Token = await UserManager.GeneratePasswordResetTokenAsync(user),
                NewPassword = "1234aAuuuu"
            };

            await handler.Handle(input, CancellationToken.None);

            (await UserManager.CheckPasswordAsync(user, "1234aAuuuu")).Should().BeTrue();
        }

        [Fact]
        public async Task ThrowsIfUnknownUser()
        {
            var input = new ResetPassword.Input {
                EmailAddress = "unknown@example.com",
                Token = await UserManager.GeneratePasswordResetTokenAsync(user),
                NewPassword = "1234aAuuuu"
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<IdentityResultException>();
        }

        [Fact]
        public async Task ThrowsIfInvalidToken()
        {
            var input = new ResetPassword.Input {
                EmailAddress = "test@example.com",
                Token = "bad-token",
                NewPassword = "1234aAuuuu"
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<IdentityResultException>();
        }

        [Fact]
        public async Task ThrowsIfInvalidPassword()
        {
            var input = new ResetPassword.Input
            {
                EmailAddress = "test@example.com",
                Token = await UserManager.GeneratePasswordResetTokenAsync(user),
                NewPassword = "password"
            };

            await F(() => handler.Handle(input, CancellationToken.None))
                .Should().ThrowAsync<IdentityResultException>();
        }
    }
}