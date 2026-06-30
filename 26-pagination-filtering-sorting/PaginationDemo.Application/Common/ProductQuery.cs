namespace PaginationDemo.Application.Common;

/// <summary>
/// Ürün sorgu parametreleri — filtering, sorting ve pagination bir arada.
/// Query string'den otomatik bind edilir.
/// </summary>
public class ProductQuery
{
    /// <summary>Sayfa numarası — 1'den başlar</summary>
    public int Page { get; set; } = 1;

    /// <summary>Sayfa başına kayıt — max 100</summary>
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 100 ? 100 : value; // Max 100 ile sınırla
    }

    /// <summary>İsme göre arama</summary>
    public string? Search { get; set; }

    /// <summary>Kategoriye göre filtrele</summary>
    public string? Category { get; set; }

    /// <summary>Minimum fiyat</summary>
    public decimal? MinPrice { get; set; }

    /// <summary>Maximum fiyat</summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>Sıralama alanı — name, price, createdat, stock</summary>
    public string SortBy { get; set; } = "id";

    /// <summary>Sıralama yönü — asc veya desc</summary>
    public string SortOrder { get; set; } = "asc";
}