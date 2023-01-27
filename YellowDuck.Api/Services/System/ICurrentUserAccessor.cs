using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Enums;
using System.Threading.Tasks;

namespace YellowDuck.Api.Services.System
{
    public interface ICurrentUserAccessor
    {
        string GetCurrentUserId();
        ValueTask<AppUser> GetCurrentUser();
        bool IsUserType(UserType type);
    }
}
