using Chameleon.Payments.Stripe.Dto;

namespace Chameleon.Payments.Stripe
{
    public class StripePaymentAppService 
        : ChameleonAppServiceBase
        , IStripePaymentAppService
    {
        private readonly StripePaymentGatewayConfiguration _stripeStripePaymentGatewayConfiguration;

        public StripePaymentAppService(
            StripePaymentGatewayConfiguration stripeStripePaymentGatewayConfiguration
            )
        {
            _stripeStripePaymentGatewayConfiguration = stripeStripePaymentGatewayConfiguration;
        }

        public StripeConfigurationDto GetConfiguration()
        {
            return new StripeConfigurationDto
            {
                PublishableKey = _stripeStripePaymentGatewayConfiguration.PublishableKey
            };
        }
    }
}