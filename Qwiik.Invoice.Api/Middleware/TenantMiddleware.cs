namespace Qwiik.Invoice.Api.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
        {
            const string tenantHeader = "X-Tenant-Id";
            if (!context.Request.Headers.TryGetValue(tenantHeader, out var tenantIdValue))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("X-Tenant-Id header is required.");
                return;
            }
            if (!int.TryParse(tenantIdValue, out var tenantId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid Tenant Id.");
                return;
            }
            tenantContext.TenantId = tenantId;
            await _next(context);
        }
    }
}