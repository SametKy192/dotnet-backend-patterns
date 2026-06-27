namespace FeatureFlags.Api.Features;

/// <summary>
/// Feature flag isimleri — magic string yerine constant kullan.
/// appsettings.json'daki isimlerle eşleşmeli.
/// </summary>
public static class FeatureNames
{
    /// <summary>Yeni ödeme sayfası — A/B test için</summary>
    public const string NewCheckout = "NewCheckout";

    /// <summary>Yapay zeka destekli ürün önerileri</summary>
    public const string AiRecommendations = "AiRecommendations";

    /// <summary>Dark mode desteği</summary>
    public const string DarkMode = "DarkMode";

    /// <summary>Beta kullanıcıları için erken erişim</summary>
    public const string BetaFeature = "BetaFeature";
}