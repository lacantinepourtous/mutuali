using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {

        private readonly IMailer mailer;
        private readonly IConfiguration config;
        private readonly ILogger<ContactController> logger;


        public ContactController(IMailer mailer, IConfiguration config, ILogger<ContactController> logger)
        {
            this.mailer = mailer;
            this.config = config;
            this.logger = logger;
        }
        public async Task<IActionResult> Post(ContactRequest contactRequest)
        {
            await mailer.Send(new ContactEmail(config["contactEmailRecipient"]) {
                FullName = contactRequest.FullName,
                OrganizationName = contactRequest.OrganizationName,
                EmailOrPhone = contactRequest.EmailOrPhone,
                Origin = contactRequest.Origin
            });
            logger.LogInformation($"Contact email sent");

            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status204NoContent
            };
        }
    }

    public class ContactRequest
    {
        public string FullName { get; set; }
        public string OrganizationName { get; set; }
        public string EmailOrPhone { get; set; }
        public string Origin { get; set; }
    }
}