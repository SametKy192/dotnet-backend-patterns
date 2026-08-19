using System.Text.Json;
using GlobalExceptionHandling.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionHandling.Api.Middleware;

/// <summary>
/// Global exception handler — tüm exception'ları yakalar.
/// Controller'larda try-catch yazmaya gerek kalmaz.
/// RFC 7807 Problem Details formatında hata döndürür.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Hata logla
        _logger.LogError(exception,
            "Exception yakalandı: {ExceptionType} — {Message}",
            exception.GetType().Name,
            exception.Message);

        // RFC 7807 Problem Details
        var problemDetails = exception switch
        {
            // Custom exception'larımız
            NotFoundException ex => new ProblemDetails
            {
                Status = ex.StatusCode,
                Title = "Kayıt Bulunamadı",
                Detail = ex.Message,
                Type = "https://tools.ietf.org/html/rfc7807#section-3",
                Extensions = { ["errorCode"] = ex.ErrorCode }
            },

            Application.Exceptions.ValidationException ex => new ProblemDetails
            {
                Status = ex.StatusCode,
                Title = "Validation Hatası",
                Detail = ex.Message,
                Type = "https://tools.ietf.org/html/rfc7807#section-3",
                Extensions =
                {
                    ["errorCode"] = ex.ErrorCode,
                    ["errors"] = ex.Errors
                }
            },

            ConflictException ex => new ProblemDetails
            {
                Status = ex.StatusCode,
                Title = "Çakışma Hatası",
                Detail = ex.Message,
                Type = "https://tools.ietf.org/html/rfc7807#section-3",
                Extensions = { ["errorCode"] = ex.ErrorCode }
            },

            ForbiddenException ex => new ProblemDetails
            {
                Status = ex.StatusCode,
                Title = "Yetkisiz Erişim",
                Detail = ex.Message,
                Type = "https://tools.ietf.org/html/rfc7807#section-3",
                Extensions = { ["errorCode"] = ex.ErrorCode }
            },

            // Beklenmedik exception'lar — detay gösterme
            _ => new ProblemDetails
            {
                Status = 500,
                Title = "Sunucu Hatası",
                Detail = "Beklenmedik bir hata oluştu.",
                Type = "https://tools.ietf.org/html/rfc7807#section-3",
                Extensions = { ["errorCode"] = "INTERNAL_SERVER_ERROR" }
            }
        };

        // Request path ekle
        problemDetails.Instance = context.Request.Path;

        // TraceId/CorrelationId ekle
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = problemDetails.Status ?? 500;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // true döndür — exception handle edildi
        return true;
    }
}