using Chameleon.Controllers;
using Chameleon.Payments.Stripe;

namespace Chameleon.Web.Host.Controllers
{
    public class StripeController : StripeControllerBase
    {
        public StripeController(
            StripeGatewayManager stripeGatewayManager,
            StripePaymentGatewayConfiguration stripeConfiguration)
            : base(stripeGatewayManager, stripeConfiguration)
        {
        }
    }
}
