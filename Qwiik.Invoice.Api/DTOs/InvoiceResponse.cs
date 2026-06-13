using Qwiik.Invoice.Api.Entities;

namespace Qwiik.Invoice.Api.DTOs
{
    public class InvoiceResponse
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public InvoiceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}