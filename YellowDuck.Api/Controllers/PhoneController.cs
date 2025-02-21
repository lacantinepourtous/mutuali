using YellowDuck.Api.DbModel.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.Services.Twilio;
using Microsoft.AspNetCore.Http;

namespace YellowDuck.Api.Controllers
{
    [Route("phone")]
    public class PhoneController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<TokenController> logger;
        private readonly AppDbContext _context;
        private readonly ITwilioService _twilioService;

        public PhoneController(
            UserManager<AppUser> userManager,
            ILogger<TokenController> logger,
            AppDbContext context,
            ITwilioService twilioService)
        {
            this.userManager = userManager; // TODO retiré si pas utilisé
            this.logger = logger;
            _context = context;
            _twilioService = twilioService;
        }

        [HttpPost("verify-request")]
        public async Task<IActionResult> VerifyRequest([FromBody] PhoneVerifyRequestRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var phone = request.PhoneNumberOrEmail;
            if (request.PhoneNumberOrEmail.Contains("@"))
            {
                var user = await _context.Users.Include(x => x.Profile).FirstOrDefaultAsync(x => x.Email == request.PhoneNumberOrEmail);
                if (user == null)
                {
                    return BadRequest(new PhoneResponse
                    {
                        Success = false,
                        MessageKey = "error.phone.email-not-found"
                    });
                }
                phone = user.Profile.PhoneNumber;
            }

            var existingVerification = await _context.PhoneVerifications
                .FirstOrDefaultAsync(x => x.PhoneNumber == phone);

            if (existingVerification != null)
            {
                var timeSinceCreation = DateTime.UtcNow - existingVerification.CreatedAt;
                if (timeSinceCreation.TotalSeconds < 30)
                {
                    return BadRequest(new PhoneResponse
                    {
                        Success = false,
                        MessageKey = "error.phone.wait-between-codes"
                    });
                }
                _context.PhoneVerifications.Remove(existingVerification);
            }

            // Créer une nouvelle vérification
            var code = PhoneVerification.GenerateCode();
            var verification = new PhoneVerification(phone, code);
            await _context.PhoneVerifications.AddAsync(verification);
            await _context.SaveChangesAsync();

            // Envoyer le SMS via Twilio
            try
            {
                await _twilioService.SendVerificationCode(phone, code);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'envoi du SMS");
                return StatusCode(500, new PhoneResponse
                {
                    Success = false,
                    MessageKey = "error.phone.sms-failed"
                });
            }

            return Ok(new PhoneResponse { Success = true });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] PhoneVerifyRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var phone = request.PhoneNumberOrEmail;
            if (request.PhoneNumberOrEmail.Contains("@"))
            {
                var user = await _context.Users.Include(x => x.Profile).FirstOrDefaultAsync(x => x.Email == request.PhoneNumberOrEmail);
                if (user == null)
                {
                    return BadRequest(new PhoneResponse
                    {
                        Success = false,
                        MessageKey = "error.phone.email-not-found"
                    });
                }
                phone = user.Profile.PhoneNumber;
            }

            var verification = await _context.PhoneVerifications
                .FirstOrDefaultAsync(x => x.PhoneNumber == phone && !x.IsVerified);

            if (verification == null)
            {
                return BadRequest(new PhoneResponse
                {
                    Success = false,
                    MessageKey = "error.phone.no-verification"
                });
            }

            if (verification.ExpirationTime < DateTime.UtcNow)
            {
                _context.PhoneVerifications.Remove(verification);
                await _context.SaveChangesAsync();
                return BadRequest(new PhoneResponse
                {
                    Success = false,
                    MessageKey = "error.phone.code-expired"
                });
            }

            if (verification.AttemptCount >= 3)
            {
                _context.PhoneVerifications.Remove(verification);
                await _context.SaveChangesAsync();
                return BadRequest(new PhoneResponse
                {
                    Success = false,
                    MessageKey = "error.phone.too-many-attempts"
                });
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Code, verification.VerificationCodeHash))
            {
                verification.AttemptCount++;
                await _context.SaveChangesAsync();
                return BadRequest(new PhoneResponse
                {
                    Success = false,
                    MessageKey = "error.phone.invalid-code"
                });
            }

            // Code valide
            verification.IsVerified = true;
            await _context.SaveChangesAsync();

            Response.Cookies.Append("bypass2FA", "true", new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(15)
            });

            return Ok(new PhoneResponse { Success = true });
        }

        public class PhoneVerifyRequestRequest
        {
            [Required]
            public string PhoneNumberOrEmail { get; set; }
        }

        public class PhoneVerifyRequest
        {
            [Required]
            public string Code { get; set; }
            public string PhoneNumberOrEmail { get; set; }
        }

        public class PhoneResponse
        {
            public bool Success { get; set; }
            public string MessageKey { get; set; }
        }
    }
}
