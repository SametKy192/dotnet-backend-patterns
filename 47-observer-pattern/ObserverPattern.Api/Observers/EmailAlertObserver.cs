using ObserverPattern.Api.Models;

namespace ObserverPattern.Api.Observers;

/// <summary>
/// Sends an email alert when a stock price changes by more than 5%.
/// Severity escalates to Critical for moves >= 10%.
/// </summary>
public class EmailAlertObserver : IStockObserver
{
    private const decimal Threshold = 5m;
    private readonly List<StockAlert> _alerts = [];

    public string Name => "EmailAlert";
    public IReadOnlyList<StockAlert> Alerts => _alerts;

    public Task UpdateAsync(StockPrice price)
    {
        if (Math.Abs(price.ChangePercent) < Threshold)
            return Task.CompletedTask;

        var severity  = Math.Abs(price.ChangePercent) >= 10m ? AlertSeverity.Critical : AlertSeverity.Warning;
        var direction = price.IsUp ? "📈 UP" : "📉 DOWN";

        _alerts.Add(new StockAlert(
            Name,
            price.Symbol,
            $"[Email] {price.Symbol} moved {direction} {price.ChangePercent:+0.##;-0.##}% " +
            $"to ${price.Price:F2} (prev ${price.PreviousPrice:F2})",
            severity,
            DateTime.UtcNow));

        return Task.CompletedTask;
    }
}
