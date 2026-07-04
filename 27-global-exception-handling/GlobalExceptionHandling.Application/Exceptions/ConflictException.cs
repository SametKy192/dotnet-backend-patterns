namespace GlobalExceptionHandling.Application.Exceptions;

/// <summary>
/// Çakışma hatası — 409 Conflict
/// Örn: aynı email ile kayıt
/// </summary>
public class ConflictException : BaseException
{
    public ConflictException(string message)
        : base(message, 409, "CONFLICT")
    {
    }
}