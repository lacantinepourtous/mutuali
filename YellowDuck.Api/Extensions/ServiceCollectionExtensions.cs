using System;
using YellowDuck.Api.Services.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YellowDuck.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFileManager(this IServiceCollection services, IConfiguration config)
        {
            var fileManagerType = config.GetValue<string>("type");

            switch (fileManagerType)
            {
                case "azureStorage":
                    services.Configure<AzureStorageFileManagerOptions>(config.GetSection("azureStorage"));
                    services.AddTransient<IFileManager, AzureStorageFileManager>();
                    break;
                case "local":
                    services.AddTransient<IFileManager, LocalFileManager>();
                    break;
                default:
                    throw new Exception($"Unknown IFileManager type: {fileManagerType}");
            }
        }
    }
}