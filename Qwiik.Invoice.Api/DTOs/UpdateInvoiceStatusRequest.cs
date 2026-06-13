using Qwiik.Invoice.Api.Entities;

namespace Qwiik.Invoice.Api.DTOs
{
    public class UpdateInvoiceStatusRequest
    {
        public InvoiceStatus Status { get; set; }
    }
}