using Microsoft.EntityFrameworkCore;
using Qwiik.Invoice.Api.Data;
using Qwiik.Invoice.Api.DTOs;
using Qwiik.Invoice.Api.Entities;
using Qwiik.Invoice.Api.Exceptions;
using Qwiik.Invoice.Api.Middleware;
using Qwiik.Invoice.Api.Services.Interfaces;
using InvoiceEntity = Qwiik.Invoice.Api.Entities.Invoice;

namespace Qwiik.Invoice.Api.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TenantContext _tenantContext;

        public InvoiceService(ApplicationDbContext dbContext, TenantContext tenantContext)
        {
            _dbContext = dbContext;
            _tenantContext = tenantContext;
        }

        public async Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request)
        {
            ValidateCreateInvoiceRequest(request);
            var invoiceExists = await _dbContext.Invoices
                .AnyAsync(x => x.TenantId == _tenantContext.TenantId && x.InvoiceNumber == request.InvoiceNumber);

            if (invoiceExists)
                throw new ConflictException($"Invoice '{request.InvoiceNumber}' already exists.");

            var invoice = new InvoiceEntity
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                InvoiceNumber = request.InvoiceNumber,
                CustomerName = request.CustomerName,
                Amount = request.Amount,
                InvoiceDate = request.InvoiceDate,
                DueDate = request.DueDate,
                Status = InvoiceStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Invoices.Add(invoice);
            await _dbContext.SaveChangesAsync();
            return MapToResponse(invoice);
        }

        public async Task<PagedResult<InvoiceResponse>> GetInvoicesAsync(int page,int pageSize)
        {
            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 20;

            if (pageSize > 100)
                pageSize = 100;

            var query = _dbContext.Invoices.Where(x => x.TenantId == _tenantContext.TenantId);
            var totalRecords = await query.CountAsync();
            var invoices = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<InvoiceResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Items = invoices.Select(MapToResponse).ToList()
            };
        }

        public async Task<InvoiceResponse?> GetInvoiceByIdAsync(Guid invoiceId)
        {
            var invoice = await _dbContext.Invoices
                .FirstOrDefaultAsync(x =>
                    x.Id == invoiceId &&
                    x.TenantId == _tenantContext.TenantId);

            if (invoice == null)
                return null;
            return MapToResponse(invoice);
        }

        public async Task UpdateStatusAsync(Guid invoiceId, UpdateInvoiceStatusRequest request)
        {
            var invoice = await _dbContext.Invoices
                .FirstOrDefaultAsync(x => x.Id == invoiceId && x.TenantId == _tenantContext.TenantId);

            if (invoice == null)
                throw new NotFoundException("Invoice not found.");

            ValidateStatusTransition(invoice.Status, request.Status);
            invoice.Status = request.Status;
            invoice.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        private static void ValidateCreateInvoiceRequest(CreateInvoiceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.InvoiceNumber))
                throw new ValidationException("Invoice Number is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerName))
                throw new ValidationException("Customer Name is required.");

            if (request.Amount <= 0)
                throw new ValidationException("Amount must be greater than zero.");

            if (request.DueDate < request.InvoiceDate)
                throw new ValidationException("Due Date cannot be less than Invoice Date.");
        }

        private static void ValidateStatusTransition(InvoiceStatus currentStatus,InvoiceStatus newStatus)
        {
            if (currentStatus == InvoiceStatus.Paid)
                throw new ValidationException("Paid invoice cannot be modified.");

            if (currentStatus == InvoiceStatus.Cancelled)
                throw new ValidationException("Cancelled invoice cannot be modified.");
        }

        private static InvoiceResponse MapToResponse(InvoiceEntity invoice)
        {
            return new InvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerName = invoice.CustomerName,
                Amount = invoice.Amount,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                Status = invoice.Status,
                CreatedAt = invoice.CreatedAt
            };
        }
        public async Task<DashboardResponse> GetDashboardAsync()
        {
            var invoices = await _dbContext.Invoices.Where(x => x.TenantId == _tenantContext.TenantId).ToListAsync();
            return new DashboardResponse
            {
                TotalInvoices = invoices.Count,
                DraftInvoices = invoices.Count(x => x.Status == InvoiceStatus.Draft),
                PendingInvoices = invoices.Count(x => x.Status == InvoiceStatus.Pending),
                PaidInvoices = invoices.Count(x => x.Status == InvoiceStatus.Paid),
                CancelledInvoices = invoices.Count(x => x.Status == InvoiceStatus.Cancelled),
                TotalInvoiceAmount = invoices.Sum(x => x.Amount)
            };
        }
    }
}