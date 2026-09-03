using MediatR;
using System.Diagnostics;

namespace PipelineBehavior.Api.Behaviors;

/// <summary>
/// Logs request start/end and elapsed time for every MediatR request.
/// Also warns when a request takes longer than <see cref="SlowRequestThresholdMs"/> ms.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int SlowRequestThresholdMs = 500;

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("[Pipeline] Handling {RequestName}", requestName);

        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                "[Pipeline] Slow request detected: {RequestName} took {ElapsedMs}ms",
                requestName,
                sw.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogInformation(
                "[Pipeline] Handled {RequestName} in {ElapsedMs}ms",
                requestName,
                sw.ElapsedMilliseconds);
        }

        return response;
    }
}
