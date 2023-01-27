using System;
using YellowDuck.Api.Services.Crypto;
using Microsoft.AspNetCore.Http;

namespace YellowDuck.Api.Services.Files
{
    public class FileUrlProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISignatureService signature;

        public FileUrlProvider(IHttpContextAccessor httpContextAccessor, ISignatureService signature)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.signature = signature;
        }

        public string GetFileUrl(string container, string fileId, TimeSpan validFor)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return null;

            var fileUrl = signature.SignUrl($"/download/file/{container}/{fileId}", validFor);

            var request = httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            
            return $"{baseUrl}{fileUrl}";
        }
    }
}