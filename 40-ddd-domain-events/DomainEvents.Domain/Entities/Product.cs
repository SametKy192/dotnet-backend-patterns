using DomainEvents.Domain.Common;
using DomainEvents.Domain.Events;

namespace DomainEvents.Domain.Entities;

public class Product : Entity
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public Product(int id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;

        // Raise the domain event
        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price));
    }
}
