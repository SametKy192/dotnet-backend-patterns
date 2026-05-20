using JwtAuth.Application.Models;

namespace JwtAuth.Application.Interfaces;

/// <summary>
/// Token servis interface'i — JWT ve refresh token işlemleri
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// JWT access token üretir — kısa ömürlü (15 dakika)
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Refresh token üretir — uzun ömürlü (7 gün)
    /// </summary>
    string GenerateRefreshToken();
}