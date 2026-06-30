namespace PaginationDemo.Application.Common;

/// <summary>
/// Sayfalanmış sonuç — her pagination response'u bu şablonu kullanır.
/// Metadata ile birlikte döner — toplam kayıt, toplam sayfa vs.
/// </summary>
public class PagedResult<T>
{
    /// <summary>Mevcut sayfadaki veriler</summary>
    public List<T> Items { get; set; } = new();

    /// <summary>Toplam kayıt sayısı</summary>
    public int TotalCount { get; set; }

    /// <summary>Mevcut sayfa numarası</summary>
    public int Page { get; set; }

    /// <summary>Sayfa başına kayıt sayısı</summary>
    public int PageSize { get; set; }

    /// <summary>Toplam sayfa sayısı</summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>Önceki sayfa var mı</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Sonraki sayfa var mı</summary>
    public bool HasNextPage => Page < TotalPages;
}