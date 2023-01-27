using System;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.Services.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace YellowDuck.Api.Plugins.ImageSharp
{
    public class AppImageProvider : IImageProvider
    {
        private readonly IOptions<AppImageProviderOptions> options;
        private readonly IFileManager fileManager;

        public AppImageProvider(IOptions<AppImageProviderOptions> options, IFileManager fileManager)
        {
            this.options = options;
            this.fileManager = fileManager;
        }

        public bool IsValidRequest(HttpContext context)
        {
            return context.Request.Path.ToString().StartsWith(options.Value.RequestPrefix);
        }

        public async Task<IImageResolver> GetAsync(HttpContext context)
        {
            var fileId = context.Request.Path.ToString().Substring(options.Value.RequestPrefix.Length);
            
            if (await fileManager.GetFileAttributes(FileContainers.Images, fileId) == null) return null;

            return new AppImageResolver(fileManager, FileContainers.Images, fileId);
        }

        public Func<HttpContext, bool> Match { get; set; } = _ => true;
        public ProcessingBehavior ProcessingBehavior => ProcessingBehavior.All;
    }
}