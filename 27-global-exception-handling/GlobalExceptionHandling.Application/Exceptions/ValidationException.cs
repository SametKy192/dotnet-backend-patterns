namespace GlobalExceptionHandling.Application.Exceptions;

/// <summary>
/// Validation hatası — 400 Bad Request
/// </summary>
public class ValidationException : BaseException
{
    /// <summary>Alan bazlı hata mesajları</summary>
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Bir veya daha fazla validation hatası oluştu.", 400, "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string field, string message)
        : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }
}