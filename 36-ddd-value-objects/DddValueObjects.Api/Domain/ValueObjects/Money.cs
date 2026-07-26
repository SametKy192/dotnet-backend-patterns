namespace DddValueObjects.Api.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency must be specified.");
        }

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public static Money Zero(string currency) => new(0, currency);

    // Operator overloads for DDD style safety
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot add money with different currencies: '{left.Currency}' and '{right.Currency}'");
        }

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot subtract money with different currencies: '{left.Currency}' and '{right.Currency}'");
        }

        if (left.Amount < right.Amount)
        {
            throw new InvalidOperationException("Insufficient amount for subtraction.");
        }

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public override string ToString() => $"{Amount} {Currency}";
}
