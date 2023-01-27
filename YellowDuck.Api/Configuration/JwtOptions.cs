using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace YellowDuck.Api.Configuration
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = "default-issuer";
        public string Audience { get; set; } = "default-audience";
        public double TokenLifespanInMinutes { get; set; } = 5;
        public double RefreshLifespanInHours { get; set; } = 24;
        public string Secret { get; set; } = "ultrasecretyellowducksecret";

        public TokenValidationParameters GetTokenValidationParameters(bool validateLifetime = true)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,

                ValidIssuer = Issuer,
                ValidAudience = Audience,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = CreateSecurityKey()
            };
        }

        public SecurityKey CreateSecurityKey()
        {
            // TODO: Considérer d'utiliser un certificat X509 en pro
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }
    }
}
