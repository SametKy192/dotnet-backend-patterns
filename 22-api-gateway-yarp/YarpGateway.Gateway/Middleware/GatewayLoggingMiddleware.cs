namespace YarpGateway.Gateway.Middleware;

/// <summary>
/// Gateway logging middleware — tüm istekleri loglar.
/// Gateway'den geçen her isteği merkezi olarak izler.
/// </summary>
public class GatewayLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayLoggingMiddleware> _logger;

    public GatewayLoggingMiddleware(RequestDelegate next, ILogger<GatewayLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation(
            "Gateway isteği: {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "Gateway yanıtı: {Method} {Path} — {StatusCode} ({Elapsed}ms)",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}