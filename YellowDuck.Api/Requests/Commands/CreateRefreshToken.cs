using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Plugins.MediatR;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Requests.Commands
{
    public class CreateRefreshToken : IRequestHandler<CreateRefreshToken.Command, string>
    {
        private readonly UserManager<AppUser> userManager;

        public CreateRefreshToken(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null) throw new UserNotFoundException($"Unable to find ant user with ID: {request.UserId}");

            var refreshToken = GenerateRefreshToken();
            var result = await userManager.SetAuthenticationTokenAsync(user, "mutuali-refresh-token", request.DeviceId, refreshToken);
            result.AssertSuccess();

            return refreshToken;
        }

        private string GenerateRefreshToken()
        {
            var buffer = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }

            return Convert.ToBase64String(buffer);
        }

        public class UserNotFoundException : RequestValidationException
        {
            public UserNotFoundException(string message) : base(message)
            {
            }
        }

        public class Command : IRequest<string>, ISafeToRetry
        {
            public Command(string userId, string deviceId)
            {
                UserId = userId;
                DeviceId = deviceId;
            }

            public string UserId { get; set; }
            public string DeviceId { get; set; }
        }
    }
}
