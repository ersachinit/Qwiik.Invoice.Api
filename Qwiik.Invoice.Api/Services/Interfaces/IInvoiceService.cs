using Qwiik.Invoice.Api.DTOs;

namespace Qwiik.Invoice.Api.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request);
        Task<PagedResult<InvoiceResponse>> GetInvoicesAsync(int page, int pageSize);
        Task<InvoiceResponse?> GetInvoiceByIdAsync(Guid invoiceId);
        Task UpdateStatusAsync(Guid invoiceId, UpdateInvoiceStatusRequest request);
        Task<DashboardResponse> GetDashboardAsync();
    }
}