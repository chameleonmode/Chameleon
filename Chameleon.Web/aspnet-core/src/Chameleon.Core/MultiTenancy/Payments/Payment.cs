using Abp.Domain.Entities.Auditing;

namespace Chameleon.MultiTenancy.Payments
{
    public class Payment : FullAuditedEntity<long>
    {
        public Payment()
        {
            Status = PaymentStatus.NotPaid;
        }

        public string Description { get; set; }

        public PaymentGatewayType Gateway { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; protected set; }

        public string ExternalPaymentId { get; set; }

        public string InvoiceNo { get; set; }

        public string SuccessUrl { get; set; }

        public string ErrorUrl { get; set; }

        public void SetAsCancelled()
        {
            if (Status == PaymentStatus.NotPaid)
            {
                Status = PaymentStatus.Cancelled;
            }
        }

        public void SetAsFailed()
        {
            Status = PaymentStatus.Failed;
        }

        public void SetAsPaid()
        {
            if (Status == PaymentStatus.NotPaid)
            {
                Status = PaymentStatus.Paid;
            }
        }

        public void SetAsCompleted()
        {
            if (Status == PaymentStatus.Paid)
            {
                Status = PaymentStatus.Completed;
            }
        }
    }
}
