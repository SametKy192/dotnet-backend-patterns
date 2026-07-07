namespace IdempotencyPattern.Infrastructure.Models;

/// <summary>
/// Idempotency kaydı — aynı isteğin tekrar geldiğinde
/// önceki response'u döndürmek için saklanır.
/// </summary>
public class IdempotencyRecord
{
    /// <summary>
    /// Idempotency key — client'ın gönderdiği benzersiz key.
    /// Genellikle UUID/GUID formatında.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Önceki response body — JSON olarak saklanır
    /// </summary>
    public string ResponseBody { get; set; } = string.Empty;

    /// <summary>
    /// Önceki HTTP status kodu
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// İlk isteğin zamanı
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ne kadar süre saklanacak
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}