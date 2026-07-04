namespace GlobalExceptionHandling.Application.Exceptions;

/// <summary>
/// Kayıt bulunamadı — 404 Not Found
/// </summary>
public class NotFoundException : BaseException
{
    public NotFoundException(string resource, object id)
        : base($"{resource} bulunamadı: {id}", 404, "NOT_FOUND")
    {
    }

    public NotFoundException(string message)
        : base(message, 404, "NOT_FOUND")
    {
    }
}