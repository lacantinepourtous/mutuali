using MediatR;
using Stripe.Checkout;

namespace YellowDuck.Api.Requests.Events
{
    public class CompleteSession : INotification
    {
        public Session Session { get; set; }
    }
}
