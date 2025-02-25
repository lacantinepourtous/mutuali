using YellowDuck.Api.Configuration;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Requests.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YellowDuck.Api.Requests.Commands.Mutations.Accounts;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.Services.Phone;
using System;

namespace YellowDuck.Api.Controllers
{
    [Route("token")]
    public class TokenController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IOptions<JwtOptions> jwtOptions;
        private readonly IOptions<JwtBearerOptions> jwtBearerOptions;
        private readonly UserManager<AppUser> userManager;
        private readonly IUserClaimsPrincipalFactory<AppUser> claimsPrincipalFactory;
        private readonly IClock clock;
        private readonly ILogger<TokenController> logger;
        private readonly IMediator mediator;
        private readonly IPhoneVerificationService _phoneVerificationService;

        public TokenController(
            AppDbContext context,
            IOptions<JwtOptions> jwtOptions,
            IOptions<JwtBearerOptions> jwtBearerOptions,
            UserManager<AppUser> userManager,
            IUserClaimsPrincipalFactory<AppUser> claimsPrincipalFactory,
            IClock clock,
            ILogger<TokenController> logger,
            IMediator mediator,
            IPhoneVerificationService phoneVerificationService)
        {
            this.context = context;
            this.jwtOptions = jwtOptions;
            this.jwtBearerOptions = jwtBearerOptions;
            this.userManager = userManager;
            this.claimsPrincipalFactory = claimsPrincipalFactory;
            this.clock = clock;
            this.logger = logger;
            this.mediator = mediator;
            _phoneVerificationService = phoneVerificationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(request.Username);
            var valid = user != null && await userManager.CheckPasswordAsync(user, request.Password);

            if (!valid)
            {
                logger.LogWarning($"Failed logon for {request.Username}");
                return BadRequest("Wrong username or password");
            }

            if (!user.EmailConfirmed)
            {
                var input = new ResendConfirmationEmail.Input()
                {
                    Email = request.Username
                };

                await mediator.Send(input);
                logger.LogWarning($"Login rejected for {request.Username} (email not confirmed)");
                return BadRequest("Email not confirmed");
            }

            if (user.PhoneNumberConfirmed && user.TwoFactorEnabled)
            {
                var bypass2FAToken = Request.Cookies["bypass2FA"];
                if (bypass2FAToken == null ||
                    user.Bypass2FAToken != bypass2FAToken ||
                    !user.Bypass2FAExpirationUtc.HasValue ||
                    user.Bypass2FAExpirationUtc.Value < DateTime.UtcNow)
                {
                    // Si le token est expiré, on le nettoie
                    if (user.Bypass2FAExpirationUtc.HasValue && user.Bypass2FAExpirationUtc.Value < DateTime.UtcNow)
                    {
                        user.Bypass2FAToken = null;
                        user.Bypass2FAExpirationUtc = null;
                        await context.SaveChangesAsync();
                    }

                    var profile = context.UserProfiles.FirstOrDefault(x => x.UserId == user.Id);

                    var result = await _phoneVerificationService.CreateAndSendVerificationCode(
                        profile.PhoneNumber,
                        enforceDelayBetweenCodes: false
                    );

                    if (!result.Success)
                    {
                        return BadRequest("Error sending SMS");
                    }

                    return Ok("2FA code sent");
                }
            }

            var isLockedOut = await userManager.IsLockedOutAsync(user);

            if (isLockedOut)
            {
                logger.LogWarning($"Login rejected for {request.Username} (User is locked out)");
                return BadRequest("User is locked out");
            }

            var tokenPair = await CreateTokenPair(user, request.DeviceId);
            return Ok(new TokenResponse
            {
                Token = tokenPair.token,
                RefreshToken = tokenPair.refresh
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            ClaimsPrincipal principal;
            SecurityToken validatedToken;

            try
            {
                principal = GetJwtTokenHandler().ValidateToken(
                    request.Token,
                    jwtOptions.Value.GetTokenValidationParameters(validateLifetime: false),
                    out validatedToken);
            }
            catch (SecurityTokenValidationException)
            {
                return Unauthorized(); // Invalid JWT token
            }

            if (principal?.Identity == null || principal.Identity.IsAuthenticated == false || validatedToken == null)
                return Unauthorized(); // Invalid JWT token

            if (validatedToken.ValidTo < (clock.GetCurrentInstant() - Duration.FromHours(jwtOptions.Value.RefreshLifespanInHours)).ToDateTimeUtc())
                return Unauthorized(); // JWT token too old

            var user = await userManager.FindByNameAsync(principal.Identity.Name);

            var isLockedOut = await userManager.IsLockedOutAsync(user);

            if (isLockedOut)
            {
                return Unauthorized(); // User is locked out
            }

            var savedRefreshToken = await userManager.GetAuthenticationTokenAsync(user, "mutuali-refresh-token", request.DeviceId);
            if (string.IsNullOrWhiteSpace(savedRefreshToken) || savedRefreshToken != request.RefreshToken)
                return Unauthorized(); // Invalid refresh token

            var tokenPair = await CreateTokenPair(user, request.DeviceId);
            return Ok(new TokenResponse
            {
                Token = tokenPair.token,
                RefreshToken = tokenPair.refresh
            });
        }

        private async Task<(string token, string refresh)> CreateTokenPair(AppUser user, string deviceId)
        {
            return (
                token: await GenerateAccessToken(user),
                refresh: await mediator.Send(new CreateRefreshToken.Command(user.Id, deviceId))
            );
        }

        private async Task<string> GenerateAccessToken(AppUser user)
        {
            var principal = await claimsPrincipalFactory.CreateAsync(user);
            var handler = GetJwtTokenHandler();

            return handler.CreateEncodedJwt(
                issuer: jwtOptions.Value.Issuer,
                audience: jwtOptions.Value.Audience,
                subject: principal.Identity as ClaimsIdentity,
                notBefore: clock.GetCurrentInstant().ToDateTimeUtc(),
                expires: (clock.GetCurrentInstant() + Duration.FromMinutes(jwtOptions.Value.TokenLifespanInMinutes)).ToDateTimeUtc(),
                issuedAt: clock.GetCurrentInstant().ToDateTimeUtc(),
                signingCredentials: new SigningCredentials(
                    jwtOptions.Value.CreateSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
        }


        private JwtSecurityTokenHandler GetJwtTokenHandler()
        {
            return jwtBearerOptions.Value.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault();
        }

        public class LoginRequest
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            public string DeviceId { get; set; }
        }

        public class RefreshRequest
        {
            [Required]
            public string Token { get; set; }
            [Required]
            public string RefreshToken { get; set; }
            [Required]
            public string DeviceId { get; set; }
        }

        public class TokenResponse
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
