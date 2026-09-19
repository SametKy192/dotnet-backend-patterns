namespace ObserverPattern.Api.Models;

public enum AlertSeverity { Info, Warning, Critical }

/// <summary>An alert record produced by an observer in response to a price update.</summary>
public record StockAlert(
    string        ObserverName,
    string        Symbol,
    string        Message,
    AlertSeverity Severity,
    DateTime      SentAt);
