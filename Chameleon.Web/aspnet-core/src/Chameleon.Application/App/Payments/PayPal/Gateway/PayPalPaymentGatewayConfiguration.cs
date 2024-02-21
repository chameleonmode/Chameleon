using Abp.Extensions;
using Microsoft.Extensions.Configuration;
using Chameleon.Configuration;
using Chameleon.MultiTenancy.Payments;

namespace Chameleon.App.Payments.PayPal
{
    public class PayPalPaymentGatewayConfiguration : IPaymentGatewayConfiguration
    {
        private readonly IConfigurationRoot _appConfiguration;

        public PaymentGatewayType GatewayType => PaymentGatewayType.Paypal;

        public const string ConfigurationKeyPrefix = "Payment:Paypal:";

        public string Environment => _appConfiguration[ConfigurationKeyPrefix + "Environment"];

        public string ClientId => _appConfiguration[ConfigurationKeyPrefix + "ClientId"];

        public string ClientSecret => _appConfiguration[ConfigurationKeyPrefix + "ClientSecret"];

        public string DemoUsername => _appConfiguration[ConfigurationKeyPrefix + "DemoUsername"];

        public string DemoPassword => _appConfiguration[ConfigurationKeyPrefix + "DemoPassword"];

        public bool IsActive => _appConfiguration[ConfigurationKeyPrefix + "IsActive"].To<bool>();

        public bool SupportsRecurringPayments => false;

        public PayPalPaymentGatewayConfiguration(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }
    }
}