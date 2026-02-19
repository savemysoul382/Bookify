using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Bookify.Api.Middleware;

public class RequestContextLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public RequestContextLoggingMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(httpContext)))
        {
            return this._next.Invoke(httpContext);
        }
    }

    private static string GetCorrelationId(HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue(
            CorrelationIdHeaderName,
            out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
    }
}