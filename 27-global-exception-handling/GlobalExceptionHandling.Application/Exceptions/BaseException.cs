namespace GlobalExceptionHandling.Application.Exceptions;

/// <summary>
/// Tüm custom exception'ların base class'ı.
/// HTTP status kodu ve hata kodu burada tanımlanır.
/// </summary>
public abstract class BaseException : Exception
{
    /// <summary>HTTP status kodu — 400, 404, 409 vs.</summary>
    public int StatusCode { get; }

    /// <summary>Makine okunabilir hata kodu — frontend için</summary>
    public string ErrorCode { get; }

    protected BaseException(string message, int statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}