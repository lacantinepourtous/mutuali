using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace YellowDuck.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGraphiQL(this IApplicationBuilder app, string path = "/graphql")
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            return app.Use(async (ctx, next) =>
            {
                if (ctx.Request.Path == "/graphql" && ctx.Request.Method == "GET")
                {
                    ctx.Response.ContentType = "text/html";
                    ctx.Response.StatusCode = 200;
                    await ctx.Response.SendFileAsync(Path.Combine(env.WebRootPath, "graphiql/index.html"));
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
