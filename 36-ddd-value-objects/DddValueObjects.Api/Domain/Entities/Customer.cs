using DddValueObjects.Api.Domain.ValueObjects;

namespace DddValueObjects.Api.Domain.Entities;

public class Customer
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Address BillingAddress { get; private set; } = null!;
    public Money Balance { get; private set; } = null!;

    // Required by EF Core
    private Customer()
    {
    }

    public Customer(string name, Email email, Address billingAddress, Money initialBalance)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Customer name is required.");
        }

        Name = name.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
        Balance = initialBalance ?? throw new ArgumentNullException(nameof(initialBalance));
    }

    // Domain business methods showing Value Objects usage
    public void Deposit(Money amount)
    {
        if (amount == null) throw new ArgumentNullException(nameof(amount));
        Balance += amount; // Enforces matching currency via Money operator overload!
    }

    public void Withdraw(Money amount)
    {
        if (amount == null) throw new ArgumentNullException(nameof(amount));
        Balance -= amount; // Enforces matching currency and balance limit via Money operator overload!
    }

    public void UpdateBillingAddress(Address newAddress)
    {
        BillingAddress = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    }
}
