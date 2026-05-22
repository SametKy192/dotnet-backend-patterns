using JwtAuth.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Api.Controllers;

/// <summary>
/// Auth controller — kayıt, giriş, token yenileme, çıkış
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Yeni kullanıcı kaydı
    /// POST /api/auth/register
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        var user = await _authService.RegisterAsync(request.Username, request.Password);
        return Ok(new { user.Id, user.Username, user.Role });
    }

    /// <summary>
    /// Giriş — access token + refresh token döner
    /// POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var (accessToken, refreshToken) = await _authService.LoginAsync(
            request.Username, request.Password);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    /// <summary>
    /// Token yenileme — refresh token ile yeni access token al
    /// POST /api/auth/refresh
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var (accessToken, refreshToken) = await _authService.RefreshAsync(request.RefreshToken);
        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    /// <summary>
    /// Çıkış — refresh token'ı iptal et
    /// POST /api/auth/logout
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
    {
        await _authService.LogoutAsync(request.RefreshToken);
        return NoContent();
    }

    /// <summary>
    /// Korumalı endpoint — sadece giriş yapmış kullanıcılar erişebilir
    /// GET /api/auth/me
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var username = User.Identity?.Name;
        var role = User.Claims
            .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        return Ok(new { Username = username, Role = role });
    }
}

public record AuthRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);