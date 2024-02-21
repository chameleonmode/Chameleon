using Abp.Dependency;
using System.Threading.Tasks;

namespace Chameleon.Payments
{
    public interface IPaymentGatewayManager
        : ITransientDependency
    {
        Task<PaymentResponse> CreateCharge(PaymentCardOption creditCard, decimal amount, string description);
    }
}
