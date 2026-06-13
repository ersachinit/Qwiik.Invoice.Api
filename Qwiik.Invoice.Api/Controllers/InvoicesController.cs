using Microsoft.AspNetCore.Mvc;
using Qwiik.Invoice.Api.DTOs;
using Qwiik.Invoice.Api.Services.Interfaces;

namespace Qwiik.Invoice.Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceResponse>> CreateInvoice(CreateInvoiceRequest request)
        {
            var invoice = await _invoiceService.CreateInvoiceAsync(request);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<InvoiceResponse>>> GetInvoices(int page = 1,int pageSize = 20)
        {
            var invoices = await _invoiceService.GetInvoicesAsync(page, pageSize);
            return Ok(invoices);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<InvoiceResponse>> GetInvoiceById(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound();
            return Ok(invoice);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateInvoiceStatusRequest request)
        {
            await _invoiceService.UpdateStatusAsync(id, request);
            return NoContent();
        }
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardResponse>> GetDashboard()
        {
            var dashboard = await _invoiceService.GetDashboardAsync();
            return Ok(dashboard);
        }
    }
}