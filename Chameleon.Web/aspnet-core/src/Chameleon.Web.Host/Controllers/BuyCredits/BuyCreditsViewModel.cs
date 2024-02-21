using System;

namespace Chameleon.Web.Host.Controllers
{
    public class BuyCreditsViewModel
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string UserEmail { get; set; }
    }
}
