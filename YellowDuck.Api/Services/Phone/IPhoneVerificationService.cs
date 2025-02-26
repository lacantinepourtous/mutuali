using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities;
using Microsoft.AspNetCore.Http;

namespace YellowDuck.Api.Services.Phone
{
  public interface IPhoneVerificationService
  {
    Task<VerificationResult> CreateAndSendVerificationCode(string phoneNumber, bool enforceDelayBetweenCodes = true);
    Task SetBypass2FATokenForUser(AppUser user, HttpResponse response, bool isLocalhost);
  }
}