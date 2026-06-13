namespace Qwiik.Invoice.Api.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public InvoiceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
