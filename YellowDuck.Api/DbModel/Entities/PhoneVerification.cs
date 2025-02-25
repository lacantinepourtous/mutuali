using System;
using System.Security.Cryptography;

namespace YellowDuck.Api.DbModel.Entities
{
    public class PhoneVerification
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string VerificationCodeHash { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsVerified { get; set; }
        public int AttemptCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PhoneVerification() { }
        public PhoneVerification(string phoneNumber, string code)
        {
            PhoneNumber = phoneNumber;
            VerificationCodeHash = BCrypt.Net.BCrypt.HashPassword(code);
            ExpirationTime = DateTime.UtcNow.AddMinutes(5);
        }

        public static string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public static (string token, DateTime expirationUtc) GenerateBypass2FAToken()
        {
            var tokenBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return (
                Convert.ToBase64String(tokenBytes),
                DateTime.UtcNow.AddDays(15)
            );
        }
    }
}
