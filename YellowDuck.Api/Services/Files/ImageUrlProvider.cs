using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Plugins.ImageSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace YellowDuck.Api.Services.Files
{
    public class ImageUrlProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOptions<AppImageProviderOptions> imageProviderOptions;

        public ImageUrlProvider(IHttpContextAccessor httpContextAccessor, IOptions<AppImageProviderOptions> imageProviderOptions)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.imageProviderOptions = imageProviderOptions;
        }
        
        // TODO: On pourrait utiliser un CDN ici
        public string GetImageUrl(string fileId, ImageFormat format = null)
        {
            if (string.IsNullOrWhiteSpace(fileId)) return null;

            var request = httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var prefix = imageProviderOptions.Value.RequestPrefix;

            return $"{baseUrl}{prefix}{fileId}{format?.GetQueryString()}";
        }
    }
}