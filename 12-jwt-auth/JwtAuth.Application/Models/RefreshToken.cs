namespace JwtAuth.Application.Models;

/// <summary>
/// Refresh token — access token süresi dolunca yenisini almak için kullanılır.
/// Veritabanında saklanır, tek kullanımlık.
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }

    /// <summary>
    /// Token değeri — güvenli rastgele string
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Hangi kullanıcıya ait
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Son kullanma tarihi — süresi geçmişse geçersiz
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Kullanıldı mı — token rotation için
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// İptal edildi mi — logout için
    /// </summary>
    public bool IsRevoked { get; set; }
}