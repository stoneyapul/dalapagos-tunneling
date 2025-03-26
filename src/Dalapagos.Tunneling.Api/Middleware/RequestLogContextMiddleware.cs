namespace Dalapagos.Tunneling.Api.Middleware;

using Serilog.Context;

public class RequestLogContextMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        {
            return next(context);
        }
    }
}