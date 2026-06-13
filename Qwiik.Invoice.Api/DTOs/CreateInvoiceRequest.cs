namespace Qwiik.Invoice.Api.DTOs
{
    public class CreateInvoiceRequest
    {
        public string InvoiceNumber { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }
    }
}