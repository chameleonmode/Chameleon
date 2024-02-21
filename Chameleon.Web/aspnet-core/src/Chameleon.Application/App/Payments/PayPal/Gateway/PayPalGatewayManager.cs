using Abp;
using Chameleon.Payments;
using System;
using System.Threading.Tasks;

namespace Chameleon.App.Payments.PayPal
{
    public class PayPalGatewayManager
        : AbpServiceBase
        , IPayPalGatewayManager
    {
        public async Task<PaymentResponse> CreateCharge(PaymentCardOption creditCard, decimal amount, string description)
        {
            throw new NotImplementedException();
        }
    }
}
