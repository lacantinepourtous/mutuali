using System.Threading.Tasks;

namespace YellowDuck.Api.Services.Phone
{
  public interface IPhoneVerificationService
  {
    Task<VerificationResult> CreateAndSendVerificationCode(string phoneNumber, bool enforceDelayBetweenCodes = true);
  }
}