using ObserverPattern.Api.Models;

namespace ObserverPattern.Api.Observers;

/// <summary>
/// Immutable audit trail — records every price update with full details.
/// Used for compliance and post-trade analysis.
/// </summary>
public class AuditLogObserver : IStockObserver
{
    private readonly List<StockAlert> _alerts = [];

    public string Name => "AuditLog";
    public IReadOnlyList<StockAlert> Alerts => _alerts;

    public Task UpdateAsync(StockPrice price)
    {
        _alerts.Add(new StockAlert(
            Name,
            price.Symbol,
            $"[Audit] {price.RecordedAt:O} | {price.Symbol} | " +
            $"prev=${price.PreviousPrice:F2} → new=${price.Price:F2} | " +
            $"Δ{price.ChangePercent:+0.##;-0.##;0}% ({price.ChangeAmount:+0.##;-0.##;0})",
            AlertSeverity.Info,
            DateTime.UtcNow));

        return Task.CompletedTask;
    }
}
