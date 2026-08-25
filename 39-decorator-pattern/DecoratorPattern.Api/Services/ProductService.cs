namespace DecoratorPattern.Api.Services;

/// <summary>
/// Core service implementation (the component being decorated).
/// Contains only pure business logic — no caching, no logging.
/// </summary>
public class ProductService : IProductService
{
    private readonly List<ProductDto> _store = new()
    {
        new(1, "Mechanical Keyboard", 89.99m, 50),
        new(2, "Wireless Mouse", 29.99m, 120),
        new(3, "4K Monitor", 499.99m, 20),
    };
    private int _nextId = 4;

    public Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = _store.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<ProductDto>>(_store.ToList());
    }

    public Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new ProductDto(_nextId++, dto.Name, dto.Price, dto.Stock);
        _store.Add(product);
        return Task.FromResult(product);
    }
}
