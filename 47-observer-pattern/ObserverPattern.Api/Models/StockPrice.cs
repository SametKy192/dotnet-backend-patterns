namespace ObserverPattern.Api.Models;

/// <summary>Represents a stock price snapshot, including change metrics.</summary>
public record StockPrice(
    string   Symbol,
    decimal  Price,
    decimal  PreviousPrice,
    DateTime RecordedAt)
{
    public decimal ChangeAmount  => Price - PreviousPrice;
    public decimal ChangePercent => PreviousPrice == 0 ? 0
        : Math.Round((Price - PreviousPrice) / PreviousPrice * 100, 2);
    public bool IsUp   => Price > PreviousPrice;
    public bool IsDown => Price < PreviousPrice;
}
