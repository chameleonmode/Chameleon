using Abp.Dependency;

namespace Chameleon.MultiTenancy.Payments
{
    public interface IPaymentGatewayConfiguration: ITransientDependency
    {
        bool IsActive { get; }

        bool SupportsRecurringPayments { get; }

        PaymentGatewayType GatewayType { get; }
    }
}
