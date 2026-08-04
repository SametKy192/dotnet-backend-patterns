namespace StronglyTypedIds.Api.Domain.Entities;

public class Customer
{
    public CustomerId Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Customer() { } // EF Core

    public Customer(CustomerId id, string name)
    {
        if (id.Value == Guid.Empty) throw new ArgumentException("Customer ID cannot be empty.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Customer name is required.");

        Id = id;
        Name = name.Trim();
    }
}
