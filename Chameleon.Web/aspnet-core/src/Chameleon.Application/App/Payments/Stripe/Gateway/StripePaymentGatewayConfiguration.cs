using Abp.Extensions;
using Microsoft.Extensions.Configuration;
using Chameleon.Configuration;
using Chameleon.MultiTenancy.Payments;

namespace Chameleon.Payments.Stripe
{
    public class StripePaymentGatewayConfiguration : IPaymentGatewayConfiguration
    {
        private readonly IConfigurationRoot _appConfiguration;

        public PaymentGatewayType GatewayType => PaymentGatewayType.Stripe;

        public const string ConfigurationKeyPrefix = "Payment:Stripe:";

        public string BaseUrl => _appConfiguration[ConfigurationKeyPrefix + "BaseUrl"].EnsureEndsWith('/');

        public string PublishableKey => _appConfiguration[ConfigurationKeyPrefix + "PublishableKey"];

        public string SecretKey => _appConfiguration[ConfigurationKeyPrefix + "SecretKey"];

        public string WebhookSecret => _appConfiguration[ConfigurationKeyPrefix + "WebhookSecret"];

        public bool IsActive => _appConfiguration[ConfigurationKeyPrefix + "IsActive"].To<bool>();
        
        public bool SupportsRecurringPayments => true;

        public StripePaymentGatewayConfiguration(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }
    }
}