namespace StronglyTypedIds.Api.Domain.Entities;

public class Order
{
    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice { get; private set; }

    private Order() { } // EF Core

    public Order(OrderId id, CustomerId customerId, ProductId productId, int quantity, decimal unitPrice)
    {
        if (id.Value == Guid.Empty) throw new ArgumentException("Order ID cannot be empty.");
        if (customerId.Value == Guid.Empty) throw new ArgumentException("Customer ID is required.");
        if (productId.Value == Guid.Empty) throw new ArgumentException("Product ID is required.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

        Id = id;
        CustomerId = customerId;
        ProductId = productId;
        Quantity = quantity;
        TotalPrice = quantity * unitPrice;
    }
}
