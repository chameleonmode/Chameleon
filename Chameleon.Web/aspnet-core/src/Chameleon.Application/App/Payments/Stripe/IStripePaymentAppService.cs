using Abp.Application.Services;
using Chameleon.Payments.Stripe.Dto;

namespace Chameleon.Payments.Stripe
{
    public interface IStripePaymentAppService : IApplicationService
    {
        StripeConfigurationDto GetConfiguration();
    }
}