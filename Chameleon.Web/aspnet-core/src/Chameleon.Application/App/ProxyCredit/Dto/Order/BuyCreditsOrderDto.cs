using System;

namespace Chameleon.App
{
    public class BuyCreditsOrderDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Url { get; set; }
    }
}
