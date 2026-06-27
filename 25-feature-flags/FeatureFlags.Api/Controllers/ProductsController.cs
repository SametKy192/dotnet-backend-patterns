using FeatureFlags.Api.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace FeatureFlags.Api.Controllers;

/// <summary>
/// Ürün controller'ı — feature flag ile endpoint'leri kontrol et
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IFeatureManager featureManager, ILogger<ProductsController> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    /// <summary>
    /// Ürün listesi — AiRecommendations açıksa AI önerileri de döner
    /// GET /api/products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 999.99 },
            new { Id = 2, Name = "Mouse", Price = 29.99 }
        };

        // Feature flag kontrolü — kod içinde
        if (await _featureManager.IsEnabledAsync(FeatureNames.AiRecommendations))
        {
            _logger.LogInformation("AI önerileri aktif — öneriler ekleniyor");

            return Ok(new
            {
                Products = products,
                AiRecommendations = new[]
                {
                    new { Id = 3, Name = "Keyboard", Price = 79.99, Reason = "AI önerisi" }
                },
                FeatureEnabled = true
            });
        }

        return Ok(new { Products = products, FeatureEnabled = false });
    }

    /// <summary>
    /// Yeni checkout sayfası — NewCheckout flag'i açıksa yeni versiyon
    /// GET /api/products/checkout
    /// </summary>
    [HttpGet("checkout")]
    public async Task<IActionResult> Checkout()
    {
        if (await _featureManager.IsEnabledAsync(FeatureNames.NewCheckout))
        {
            return Ok(new
            {
                Version = "v2",
                Message = "Yeni checkout sayfası — daha hızlı ve güvenli",
                Features = new[] { "Apple Pay", "Google Pay", "Kripto ödeme" }
            });
        }

        return Ok(new
        {
            Version = "v1",
            Message = "Klasik checkout sayfası"
        });
    }

    /// <summary>
    /// Beta özellik — [FeatureGate] attribute ile kontrol.
    /// Flag kapalıysa otomatik 404 döner, kod çalışmaz.
    /// GET /api/products/beta
    /// </summary>
    [HttpGet("beta")]
    [FeatureGate(FeatureNames.BetaFeature)]
    public IActionResult Beta()
    {
        return Ok(new
        {
            Message = "Beta özelliğe hoş geldiniz!",
            Note = "Bu endpoint sadece BetaFeature flag'i açıksa erişilebilir"
        });
    }

    /// <summary>
    /// Tüm feature flag'lerin durumunu göster
    /// GET /api/products/features
    /// </summary>
    [HttpGet("features")]
    public async Task<IActionResult> GetFeatures()
    {
        return Ok(new
        {
            NewCheckout = await _featureManager.IsEnabledAsync(FeatureNames.NewCheckout),
            AiRecommendations = await _featureManager.IsEnabledAsync(FeatureNames.AiRecommendations),
            DarkMode = await _featureManager.IsEnabledAsync(FeatureNames.DarkMode),
            BetaFeature = await _featureManager.IsEnabledAsync(FeatureNames.BetaFeature)
        });
    }
}