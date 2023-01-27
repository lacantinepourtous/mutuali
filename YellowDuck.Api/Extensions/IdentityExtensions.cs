using YellowDuck.Api.Constants;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Profiles;
using YellowDuck.Api.DbModel.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YellowDuck.Api.Extensions
{
    public static class IdentityExtensions
    {
        public static void AssertSuccess(this IdentityResult result)
        {
            if (result.Succeeded) return;
            throw new IdentityResultException(result);
        }

        public static async Task<IdentityResult> CreateOrUpdateAsync(
            this UserManager<AppUser> userManager, AppDbContext db, CreateOrUpdateIdentity identity)
        {
            var existingUser = await userManager.FindByEmailAsync(identity.Email);
            if (existingUser == null)
            {
                var acceptationDate = DateTime.Now;
                var createdAtUtcDate = DateTime.UtcNow;

                var user = new AppUser(identity.Email)
                {
                    Type = identity.Type,
                    EmailConfirmed = true,
                    Profile = new UserProfile()
                    {
                        FirstName = identity.FirstName,
                        LastName = identity.LastName,
                        OrganizationName = identity.OrganizationName,
                        OrganizationType = identity.OrganizationType,
                        Industry = identity.Industry,
                        PostalCode = identity.PostalCode,
                        PhoneNumber = identity.PhoneNumber,
                        ShowPhoneNumber = identity.ShowPhoneNumber,
                        ShowEmail = identity.ShowEmail
                    },
                    AcceptedTos = TosVersion.Latest,
                    TosAcceptationDate = acceptationDate,
                    CreatedAtUtc = createdAtUtcDate
                };
                user.Profile.User = user;

                return await userManager
                    .CreateAsync(user, identity.Password);
            }
            else
            {
                existingUser.Type = identity.Type;
                existingUser.EmailConfirmed = true;

                var result = await userManager.UpdateAsync(existingUser);

                if (result.Succeeded && !await userManager.CheckPasswordAsync(existingUser, identity.Password))
                {
                    await userManager.RemovePasswordAsync(existingUser);
                    result = await userManager.AddPasswordAsync(existingUser, identity.Password);
                }

                var profile = await db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == existingUser.Id);
                if (profile == null)
                {
                    profile = new UserProfile
                    {
                        UserId = existingUser.Id,
                        FirstName = identity.FirstName,
                        LastName = identity.LastName,
                        OrganizationName = identity.OrganizationName,
                        OrganizationType = identity.OrganizationType,
                        Industry = identity.Industry,
                        PhoneNumber = identity.PhoneNumber,
                        ShowPhoneNumber = identity.ShowPhoneNumber,
                        ShowEmail = identity.ShowEmail
                    };
                    db.UserProfiles.Add(profile);
                }

                return result;
            }
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return (principal.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static bool IsUserType(this ClaimsPrincipal principal, UserType type)
        {
            return principal.HasClaim(AppClaimTypes.UserType, type.ToString());
        }

        public static bool IsAdmin(this ClaimsPrincipal principal) => principal.IsUserType(UserType.Admin);
        public static bool IsUser(this ClaimsPrincipal principal) => principal.IsUserType(UserType.User);

        public static bool IsExpected(this IdentityResultException exception)
        {
            var hasUnexpectedErrorCode = exception.IdentityResult.Errors.Any(
                e => e.Code == nameof(IdentityErrorDescriber.ConcurrencyFailure) ||
                     e.Code == nameof(IdentityErrorDescriber.DefaultError) ||
                     e.Code == nameof(IdentityErrorDescriber.UserAlreadyHasPassword)
            );

            return !hasUnexpectedErrorCode;
        }
    }

    public class CreateOrUpdateIdentity
    {
        public string Email { get; set; }
        public UserType Type { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganizationName { get; set; }
        public OrganizationType OrganizationType { get; set; }
        public Industry Industry { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool ShowPhoneNumber { get; set; }
        public bool ShowEmail { get; set; }
    }

    [Serializable]
    public class IdentityResultException : Exception
    {
        public IdentityResult IdentityResult { get; }

        public override string Message =>
            string.Join('\n', IdentityResult.Errors.Select(err => $"{err.Code}: {err.Description}"));

        public override IDictionary Data => IdentityResult.Errors.ToDictionary(x => x.Code, x => x.Description);

        public IdentityResultException(IdentityResult result)
        {
            IdentityResult = result;
        }

        public IdentityResultException(string message) : base(message)
        {
        }

        public IdentityResultException(string message, Exception inner) : base(message, inner)
        {
        }

        protected IdentityResultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}