using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Extensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YellowDuck.Api.Services.System
{
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext db;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, AppDbContext db)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.db = db;
        }

        public string GetCurrentUserId()
        {
            return GetPrincipal().GetUserId();
        }

        public bool IsUserType(UserType type)
        {
            return GetPrincipal().HasClaim(AppClaimTypes.UserType, type.ToString());
        }

        public ValueTask<AppUser> GetCurrentUser() => db.Users.FindAsync(GetCurrentUserId());

        private ClaimsPrincipal GetPrincipal() => httpContextAccessor.HttpContext.User;
    }
}
