using Microsoft.Extensions.Configuration;
using NodaTime;
using System.Collections.Generic;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Types;
using YellowDuck.Api.DbModel.Entities;
using Twilio.Exceptions;

namespace YellowDuck.Api.Services.Twilio
{
    public class TwilioService : ITwilioService
    {
        private readonly IClock clock;
        private readonly IConfiguration configuration;

        public TwilioService(IConfiguration configuration, IClock clock)
        {
            this.configuration = configuration;
            this.clock = clock;

            TwilioClient.Init(GetAccountSid(), GetAuthToken());
        }

        public string GetAppUserToken(AppUser user)
        {
            var grant = new ChatGrant
            {
                ServiceSid = GetChatServiceSid()
            };

            var grants = new HashSet<IGrant>
            {
                { grant }
            };

            var token = new Token(
                GetAccountSid(),
                GetApiKey(),
                GetSecret(),
                user.Id,
                grants: grants,
                expiration: (clock.GetCurrentInstant() + Duration.FromHours(24)).ToDateTimeUtc());

            return token.ToJwt();
        }
        
        public string GetAccountSid()
        {
            return configuration.GetValue<string>("twilio:sid");
        }

        public string GetAuthToken()
        {
            return configuration.GetValue<string>("twilio:token");
        }

        public string GetChatServiceSid()
        {
            return configuration.GetValue<string>("twilio:serviceSid");
        }

        public string GetApiKey()
        {
            return configuration.GetValue<string>("twilio:apiKey");
        }

        public string GetSecret()
        {
            return configuration.GetValue<string>("twilio:secret");
        }

        public async Task SendVerificationCode(string phoneNumber, string code)
        {
            var message = await MessageResource.CreateAsync(
                body: $"Votre code de vérification est : {code}",
                from: new PhoneNumber(configuration.GetValue<string>("twilio:fromNumber")), // TODO - Ajouter la config.
                to: new PhoneNumber(phoneNumber)
            );

            if (message.ErrorCode != null)
            {
                throw new TwilioException(message.ErrorMessage);
            }
        }
    }
}
