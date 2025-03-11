using YellowDuck.Api.Configuration;
using YellowDuck.Api.Controllers;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using YellowDuck.Api.Requests.Commands;
using Moq;
using YellowDuck.Api.Services.Phone;
using YellowDuck.Api.DbModel;

namespace YellowDuck.ApiTests.Controllers
{
    public class TokenControllerTests : TestBase
    {
        private readonly TokenController controller;
        private readonly AppUser user;

        private const string Username = "test@example.com";
        private const string Password = "1234aAuuuuuu!";
        private const string DeviceId = "test1";

        public TokenControllerTests()
        {
            SetupRequestHandler(new CreateRefreshToken(UserManager));
            controller = new TokenController(
                context: Mock.Of<AppDbContext>(),
                jwtOptions: new OptionsWrapper<JwtOptions>(new JwtOptions()),
                jwtBearerOptions: new OptionsWrapper<JwtBearerOptions>(new JwtBearerOptions()),
                userManager: UserManager,
                claimsPrincipalFactory: new UserClaimsPrincipalFactory<AppUser>(UserManager, new OptionsWrapper<IdentityOptions>(UserManager.Options)),
                clock: Clock,
                logger: Logger<TokenController>(),
                mediator: Mediator,
                phoneVerificationService: Mock.Of<IPhoneVerificationService>());

            user = AddUser(Username, UserType.User, Password);
            user.EmailConfirmed = true;
        }

        [Fact]
        public async Task CanLoginAndReceiveTokenPair()
        {
            var response = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });

            VerifyTokenResponse(response);
        }

        [Fact]
        public async Task InvalidCredentialsReturnBadRequest()
        {
            var response = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = "0000",
                DeviceId = DeviceId
            });

            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CanRefreshWithValidTokenPair()
        {
            var loginResponse = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });

            var tokenPair = loginResponse.As<OkObjectResult>().Value.As<TokenController.TokenResponse>();

            Clock.AdvanceHours(12);

            var refreshResponse = await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair.Token,
                RefreshToken = tokenPair.RefreshToken,
                DeviceId = DeviceId
            });

            VerifyTokenResponse(refreshResponse);
        }

        [Fact]
        public async Task WrongRefreshTokenIsUnauthorized()
        {
            var loginResponse = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });

            var tokenPair = loginResponse.As<OkObjectResult>().Value.As<TokenController.TokenResponse>();

            var refreshResponse = await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair.Token,
                RefreshToken = tokenPair.RefreshToken + "bad",
                DeviceId = DeviceId
            });

            refreshResponse.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task WrongAuthTokenIsUnauthorized()
        {
            var loginResponse = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });

            var tokenPair = loginResponse.As<OkObjectResult>().Value.As<TokenController.TokenResponse>();

            var refreshResponse = await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair.Token + "bad",
                RefreshToken = tokenPair.RefreshToken,
                DeviceId = DeviceId
            });

            refreshResponse.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task TooOldAuthTokenIsUnauthorized()
        {
            var loginResponse = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });

            var tokenPair = loginResponse.As<OkObjectResult>().Value.As<TokenController.TokenResponse>();

            Clock.AdvanceHours(36);

            var refreshResponse = await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair.Token,
                RefreshToken = tokenPair.RefreshToken,
                DeviceId = DeviceId
            });

            refreshResponse.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task CantUseRefreshTokenTwice()
        {
            var loginResponse = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });

            var tokenPair = loginResponse.As<OkObjectResult>().Value.As<TokenController.TokenResponse>();

            await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair.Token,
                RefreshToken = tokenPair.RefreshToken,
                DeviceId = DeviceId
            });

            var refreshResponse = await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair.Token,
                RefreshToken = tokenPair.RefreshToken,
                DeviceId = DeviceId
            });

            refreshResponse.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task EachDeviceHasOwnRefreshToken()
        {
            var loginResponse1 = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });
            var loginResponse2 = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = "test2"
            });

            var tokenPair1 = loginResponse1.As<OkObjectResult>().Value.As<TokenController.TokenResponse>();
            var refreshResponse1 = await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair1.Token,
                RefreshToken = tokenPair1.RefreshToken,
                DeviceId = DeviceId
            });
            VerifyTokenResponse(refreshResponse1);

            var tokenPair2 = loginResponse2.As<OkObjectResult>().Value.As<TokenController.TokenResponse>();
            var refreshResponse2 = await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair2.Token,
                RefreshToken = tokenPair2.RefreshToken,
                DeviceId = "test2"
            });
            VerifyTokenResponse(refreshResponse2);
        }

        [Fact]
        public async Task RefreshTokenIsBoundToDevice()
        {
            var loginResponse = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });

            var tokenPair = loginResponse.As<OkObjectResult>().Value.As<TokenController.TokenResponse>();

            var refreshResponse = await controller.Refresh(new TokenController.RefreshRequest
            {
                Token = tokenPair.Token,
                RefreshToken = tokenPair.RefreshToken,
                DeviceId = "test2"
            });

            _ = refreshResponse.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task CantLoginIfEmailNotConfirmed()
        {
            user.EmailConfirmed = false;

            var response = await controller.Login(new TokenController.LoginRequest
            {
                Username = Username,
                Password = Password,
                DeviceId = DeviceId
            });

            _ = response.Should().BeOfType<BadRequestObjectResult>();
        }

        private static void VerifyTokenResponse(IActionResult response)
        {
            var result = response.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<TokenController.TokenResponse>()
                .Subject;

            ((Action)(() => new JwtSecurityToken(result.Token))).Should().NotThrow();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}
