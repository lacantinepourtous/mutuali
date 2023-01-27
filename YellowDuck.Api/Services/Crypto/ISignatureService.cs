using System;

namespace YellowDuck.Api.Services.Crypto
{
    public interface ISignatureService
    {
        string SignUrl(string url, TimeSpan validFor);
        bool VerifySignedUrl(string url);
    }
}