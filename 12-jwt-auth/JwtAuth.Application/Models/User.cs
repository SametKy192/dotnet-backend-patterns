namespace JwtAuth.Application.Models;

/// <summary>
/// Kullanıcı modeli — kimlik doğrulama için
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// BCrypt ile hashlenmiş şifre — düz metin saklanmaz
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";
}