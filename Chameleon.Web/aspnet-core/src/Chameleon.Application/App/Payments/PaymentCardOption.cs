namespace Chameleon.Payments
{
    public class PaymentCardOption
    {
        public string Number { get; set; }
        public long? ExpMonth { get; set; }
        public long? ExpYear { get; set; }
        public string Cvc { get; set; }
    }
}