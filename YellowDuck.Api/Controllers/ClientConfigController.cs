using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.Json;

namespace YellowDuck.Api.Controllers
{
    [Route("client-config.js")]
    public class ClientConfigController : ControllerBase
    {
        private readonly IConfiguration config;

        public ClientConfigController(IConfiguration config)
        {
            this.config = config;
        }
        public IActionResult Get()
        {
            var clientConfig = config.GetSection("clientConfig").Get<Dictionary<string, object>>();
            return Content($"window.yellowduck_env={JsonSerializer.Serialize(clientConfig)}", "text/javascript");
        }
    }
}