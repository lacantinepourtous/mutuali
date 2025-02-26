using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.Services.Phone;

namespace YellowDuck.Api.Controllers
{
    [Route("phone")]
    public class PhoneController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPhoneVerificationService _phoneVerificationService;

        public PhoneController(
            AppDbContext context,
            IPhoneVerificationService phoneVerificationService)
        {
            _context = context;
            _phoneVerificationService = phoneVerificationService;
        }

        [HttpPost("verify-request")]
        public async Task<IActionResult> VerifyRequest([FromBody] PhoneVerifyRequestRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (phone, error) = await GetPhoneNumberFromEmailOrPhone(request.PhoneNumberOrEmail);
            if (error != null) return error;

            var result = await _phoneVerificationService.CreateAndSendVerificationCode(phone);

            return result.Success
                ? Ok(new PhoneResponse { Success = true })
                : BadRequest(new PhoneResponse
                {
                    Success = false,
                    MessageKey = $"error.phone.{result.ErrorCode}"
                });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] PhoneVerifyRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (phone, error) = await GetPhoneNumberFromEmailOrPhone(request.Phone ?? request.Email);
            if (error != null) return error;

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

            // Générer un token unique pour le bypass2FA
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user != null)
            {
                var isLocalhost = Request.Host.Host.Contains("localhost");
                await _phoneVerificationService.SetBypass2FATokenForUser(user, Response, isLocalhost);
            }

            await _context.SaveChangesAsync();

            return Ok(new PhoneResponse { Success = true });
        }

        private async Task<(string phoneNumber, IActionResult error)> GetPhoneNumberFromEmailOrPhone(string phoneNumberOrEmail)
        {
            if (!phoneNumberOrEmail.Contains("@"))
                return (phoneNumberOrEmail, null);

            var user = await _context.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(x => x.Email == phoneNumberOrEmail);

            if (user == null)
            {
                return (null, BadRequest(new PhoneResponse
                {
                    Success = false,
                    MessageKey = "error.phone.email-not-found"
                }));
            }

            return (user.Profile.PhoneNumber, null);
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
            public string Phone { get; set; }
            public string Email { get; set; }
        }

        public class PhoneResponse
        {
            public bool Success { get; set; }
            public string MessageKey { get; set; }
        }
    }
}
