using ObserverPattern.Api.Models;

namespace ObserverPattern.Api.Observers;

/// <summary>
/// Tracks every price update and displays it on a dashboard.
/// Always fires, regardless of magnitude — provides a complete live feed.
/// </summary>
public class DashboardObserver : IStockObserver
{
    private readonly List<StockAlert> _alerts = [];

    public string Name => "Dashboard";
    public IReadOnlyList<StockAlert> Alerts => _alerts;

    public Task UpdateAsync(StockPrice price)
    {
        var arrow = price.IsUp ? "▲" : price.IsDown ? "▼" : "─";

        _alerts.Add(new StockAlert(
            Name,
            price.Symbol,
            $"[Dashboard] {price.Symbol}: ${price.Price:F2} {arrow} " +
            $"({price.ChangePercent:+0.##;-0.##;0}%)",
            AlertSeverity.Info,
            DateTime.UtcNow));

        return Task.CompletedTask;
    }
}
