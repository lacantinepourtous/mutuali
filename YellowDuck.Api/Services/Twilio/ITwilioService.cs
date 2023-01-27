using YellowDuck.Api.DbModel.Entities;

namespace YellowDuck.Api.Services.Twilio
{
    public interface ITwilioService
    {
        string GetAppUserToken(AppUser user);
        string GetAccountSid();
        string GetAuthToken();
        string GetChatServiceSid();
        string GetApiKey();
        string GetSecret();
    }
}
