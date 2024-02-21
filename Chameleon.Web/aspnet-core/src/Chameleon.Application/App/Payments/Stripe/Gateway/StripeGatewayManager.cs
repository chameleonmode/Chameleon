using System;
using System.Threading.Tasks;
using Abp;
using Abp.UI;
using Stripe;

namespace Chameleon.Payments.Stripe
{
    public class StripeGatewayManager 
        : AbpServiceBase
        , IStripeGatewayManager
    {
        public static string ProductName = "Chameleon";

        public async Task<PaymentResponse> CreateCharge(PaymentCardOption creditCard, decimal amount, string description)
        {
            var tokenCreationOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = creditCard.Number,
                    ExpYear = creditCard.ExpYear,
                    ExpMonth = creditCard.ExpMonth,
                    Cvc = creditCard.Cvc
                }
            };

            var tokenService = new TokenService();
            var stripeToken = await tokenService.CreateAsync(tokenCreationOptions);

            return await CreateCharge(stripeToken.Id, amount, description);
        }

        public async Task<PaymentResponse> CreateCharge(string source, decimal amount, string description)
        {
            var chargeService = new ChargeService();
            var chargeOptions = new ChargeCreateOptions
            {
                Source = source,
                Amount = ConvertToStripePrice(amount),
                Description = description,
                Currency = ChameleonConsts.Currency,
                Capture = true
            };

            var charge = await chargeService.CreateAsync(chargeOptions);

            if (!charge.Paid)
            {
                throw new UserFriendlyException(L("PaymentCouldNotCompleted"));
            }

            return new PaymentResponse
            {
                Id = charge.Id
            };
        }

        public async Task<PaymentResponse> GetOrCreateProductAsync(string productId)
        {
            try
            {
                var productService = new ProductService();
                var product = await productService.GetAsync(productId);

                return new PaymentResponse
                {
                    Id = product.Id
                };
            }
            catch (StripeException exception)
            {
                Logger.Error(exception.Message, exception);
                return await CreateProductAsync(productId);
            }
        }

        public async Task<PaymentResponse> CreateCustomerAsync(string description, string emailAddress, string source)
        {
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Description = description,
                Source = source,
                Email = emailAddress
            });

            return new PaymentResponse
            {
                Id = customer.Id
            };
        }

        private long ConvertToStripePrice(decimal amount)
        {
            return Convert.ToInt64(amount * 100);
        }

        private decimal ConvertFromStripePrice(long amount)
        {
            return Convert.ToDecimal(amount) / 100;
        }

        private async Task<PaymentResponse> CreateProductAsync(string name)
        {
            var productService = new ProductService();
            var product = await productService.CreateAsync(new ProductCreateOptions
            {
                Id = name,
                Name = name,
                Type = "service"
            });

            return new PaymentResponse
            {
                Id = product.Id
            };
        }
    }
}