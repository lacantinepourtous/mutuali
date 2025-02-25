using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.Services.Twilio;
using Microsoft.AspNetCore.Http;

namespace YellowDuck.Api.Services.Phone
{
  public class PhoneVerificationService : IPhoneVerificationService
  {
    private readonly AppDbContext _context;
    private readonly ITwilioService _twilioService;
    private readonly ILogger<PhoneVerificationService> _logger;

    public PhoneVerificationService(
        AppDbContext context,
        ITwilioService twilioService,
        ILogger<PhoneVerificationService> logger)
    {
      _context = context;
      _twilioService = twilioService;
      _logger = logger;
    }

    public async Task<VerificationResult> CreateAndSendVerificationCode(string phoneNumber, bool enforceDelayBetweenCodes = true)
    {
      var existingVerification = await _context.PhoneVerifications
          .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);

      if (existingVerification != null)
      {
        if (enforceDelayBetweenCodes)
        {
          var timeSinceCreation = DateTime.UtcNow - existingVerification.CreatedAt;
          if (timeSinceCreation.TotalSeconds < 30)
          {
            return VerificationResult.Failed("wait-between-codes");
          }
        }
        _context.PhoneVerifications.Remove(existingVerification);
      }

      var code = PhoneVerification.GenerateCode();
      var verification = new PhoneVerification(phoneNumber, code);
      await _context.PhoneVerifications.AddAsync(verification);
      await _context.SaveChangesAsync();

      try
      {
        await _twilioService.SendVerificationCode(phoneNumber, code);
        return VerificationResult.Succeeded();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de l'envoi du SMS");
        return VerificationResult.Succeeded();
      }
    }

    public async Task SetBypass2FATokenForUser(AppUser user, HttpResponse response, bool isLocalhost)
    {
      var (token, expirationUtc) = PhoneVerification.GenerateBypass2FAToken();

      user.Bypass2FAToken = token;
      user.Bypass2FAExpirationUtc = expirationUtc;

      await _context.SaveChangesAsync();

      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = !isLocalhost,
        SameSite = SameSiteMode.Strict,
        Expires = expirationUtc
      };

      response.Cookies.Append("bypass2FA", token, cookieOptions);
    }
  }
}