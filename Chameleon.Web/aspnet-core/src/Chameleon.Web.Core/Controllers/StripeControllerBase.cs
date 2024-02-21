using System;
using System.IO;
using System.Threading.Tasks;
using Chameleon.Payments.Stripe;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Chameleon.Controllers
{
    public class StripeControllerBase : ChameleonControllerBase
    {
        private readonly StripeGatewayManager _stripeGatewayManager;
        private readonly StripePaymentGatewayConfiguration _stripeConfiguration;

        public StripeControllerBase(
            StripeGatewayManager stripeGatewayManager,
            StripePaymentGatewayConfiguration stripeConfiguration)
        {
            _stripeGatewayManager = stripeGatewayManager;
            _stripeConfiguration = stripeConfiguration;
        }

        [HttpPost]
        public async Task<IActionResult> WebHooks()
        {
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();

            try
            {
                #region DEVITRUST

                var toleranceDefault = 300L;
                var throwOnApiVersionMismatch = false;

                #endregion

                var stripeEvent = EventUtility.ConstructEvent(json, 
                    Request.Headers["Stripe-Signature"], 
                    _stripeConfiguration.WebhookSecret, 
                    toleranceDefault, 
                    throwOnApiVersionMismatch
                    );

                // Other WebHook events can be handled here.

                return Ok();
            }
            catch (ApplicationException exception)
            {
                Logger.Error(exception.Message, exception);
                return BadRequest();
            }
            catch (StripeException exception)
            {
                Logger.Error(exception.Message, exception);
                return BadRequest();
            }
        }
    }
}
