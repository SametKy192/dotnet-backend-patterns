using ObserverPattern.Api.Models;

namespace ObserverPattern.Api.Observers;

/// <summary>
/// Sends an SMS alert only for large price movements (>= 10%).
/// Intended for urgent, attention-grabbing notifications.
/// </summary>
public class SmsAlertObserver : IStockObserver
{
    private const decimal Threshold = 10m;
    private readonly List<StockAlert> _alerts = [];

    public string Name => "SmsAlert";
    public IReadOnlyList<StockAlert> Alerts => _alerts;

    public Task UpdateAsync(StockPrice price)
    {
        if (Math.Abs(price.ChangePercent) < Threshold)
            return Task.CompletedTask;

        var direction = price.IsUp ? "surged" : "crashed";

        _alerts.Add(new StockAlert(
            Name,
            price.Symbol,
            $"[SMS] ⚠️ URGENT: {price.Symbol} {direction} {price.ChangePercent:+0.##;-0.##}% " +
            $"— now at ${price.Price:F2}. Act now!",
            AlertSeverity.Critical,
            DateTime.UtcNow));

        return Task.CompletedTask;
    }
}
