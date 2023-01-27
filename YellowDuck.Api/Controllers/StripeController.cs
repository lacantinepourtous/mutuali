using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Stripe;
using Microsoft.Extensions.Logging;
using MediatR;
using YellowDuck.Api.Services.Stripe;
using Stripe.Checkout;
using YellowDuck.Api.Constants;
using YellowDuck.Api.Requests.Events;

namespace YellowDuck.Api.Controllers
{
    [Route("stripe")]
    public class StripeController : ControllerBase
    {
        private readonly ILogger<StripeController> logger;
        private readonly IMediator mediator;
        private readonly IPaymentService paymentService;

        public StripeController(IPaymentService paymentService, ILogger<StripeController> logger, IMediator mediator)
        {
            this.mediator = mediator;
            this.logger = logger;
            this.paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], paymentService.GetWebhooksKey());

                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                    {

                        var session = stripeEvent.Data.Object as Session;
                        await mediator.Publish(new CompleteSession
                        {
                            Session = session
                        });
                        break;
                    }
                    case Events.CheckoutSessionAsyncPaymentFailed:
                    case Events.CheckoutSessionAsyncPaymentSucceeded:
                    case StripeEnums.CheckoutSessionExpired:
                    {
                        var session = stripeEvent.Data.Object as Session;
                        await mediator.Publish(new CheckoutSessionExpired
                        {
                            Session = session
                        });
                        break;
                    }
                    default:
                    {
                        return BadRequest();
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
