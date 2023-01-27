using MediatR;
using Stripe.Checkout;

namespace YellowDuck.Api.Requests.Events
{
    public class CheckoutSessionExpired : INotification
    {
        public Session Session { get; set; }
    }
}
