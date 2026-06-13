namespace Qwiik.Invoice.Api.DTOs
{
    public class DashboardResponse
    {
        public int TotalInvoices { get; set; }

        public int DraftInvoices { get; set; }

        public int PendingInvoices { get; set; }

        public int PaidInvoices { get; set; }

        public int CancelledInvoices { get; set; }

        public decimal TotalInvoiceAmount { get; set; }
    }
}