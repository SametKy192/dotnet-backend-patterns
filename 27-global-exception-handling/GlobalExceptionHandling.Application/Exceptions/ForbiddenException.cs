namespace GlobalExceptionHandling.Application.Exceptions;

/// <summary>
/// Yetkisiz erişim — 403 Forbidden
/// </summary>
public class ForbiddenException : BaseException
{
    public ForbiddenException(string message = "Bu işlem için yetkiniz yok.")
        : base(message, 403, "FORBIDDEN")
    {
    }
}