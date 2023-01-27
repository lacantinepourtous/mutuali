using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using YellowDuck.Api.Requests.Events;

namespace YellowDuck.Api.Controllers
{
    [Route("twilio")]
    public class TwilioController : ControllerBase
    {
        private readonly ILogger<TwilioController> logger;
        private readonly IMediator mediator;

        public TwilioController(ILogger<TwilioController> logger, IMediator mediator)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpPost("post-event")]
        public async Task<IActionResult> HandlePostEvent()
        {
            var eventType = GetValue("EventType");

            if (eventType == "onMessageAdded")
            {
                var author = GetValue("Author");
                var body = GetValue("Body");
                var conversationSid = GetValue("ConversationSid");

                await mediator.Publish(new MessageAdded
                {
                    ConversationSid = conversationSid,
                    Body = body,
                    AuthorId = author
                });
            }

            return NoContent();
        }

        private string GetValue(string key)
        {
            var form = Request.Form;
            StringValues value = new StringValues();

            form.TryGetValue(key, out value);

            return value.ToString();
        }
    }
}
