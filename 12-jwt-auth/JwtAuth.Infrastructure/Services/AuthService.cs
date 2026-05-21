using BCrypt.Net;
using JwtAuth.Application.Interfaces;
using JwtAuth.Application.Models;
using JwtAuth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Infrastructure.Services;

/// <summary>
/// Kimlik doğrulama servisi — kayıt, giriş, token yenileme, çıkış
/// </summary>
public class AuthService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Yeni kullanıcı kaydı — şifre BCrypt ile hashlenir
    /// </summary>
    public async Task<User> RegisterAsync(string username, string password)
    {
        // Kullanıcı adı zaten var mı
        if (await _dbContext.Users.AnyAsync(u => u.Username == username))
            throw new InvalidOperationException("Bu kullanıcı adı zaten kullanılıyor.");

        var user = new User
        {
            Username = username,
            // Şifre düz metin olarak saklanmaz — BCrypt hash
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Giriş — access token + refresh token döndürür
    /// </summary>
    public async Task<(string AccessToken, string RefreshToken)> LoginAsync(
        string username, string password)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username)
            ?? throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // BCrypt ile şifre doğrulama
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Şifre yanlış.");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Refresh token'ı DB'ye kaydet
        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            // 7 gün geçerli
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _dbContext.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    /// <summary>
    /// Token yenileme — refresh token ile yeni access token al.
    /// Token rotation: eski refresh token iptal edilir, yenisi verilir.
    /// </summary>
    public async Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken)
    {
        var token = await _dbContext.RefreshTokens
            .Include(t => t.UserId)
            .FirstOrDefaultAsync(t => t.Token == refreshToken)
            ?? throw new UnauthorizedAccessException("Geçersiz refresh token.");

        // Token kullanılmış mı veya iptal edilmiş mi
        if (token.IsUsed || token.IsRevoked)
            throw new UnauthorizedAccessException("Token zaten kullanılmış.");

        // Token süresi geçmiş mi
        if (token.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Token süresi dolmuş.");

        var user = await _dbContext.Users.FindAsync(token.UserId)
            ?? throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // Token rotation — eski token'ı iptal et
        token.IsUsed = true;

        // Yeni token'lar üret
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _dbContext.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    /// <summary>
    /// Çıkış — refresh token'ı iptal et
    /// </summary>
    public async Task LogoutAsync(string refreshToken)
    {
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}