namespace DecoratorPattern.Api.Services;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> CreateAsync(CreateProductDto dto);
}

public record ProductDto(int Id, string Name, decimal Price, int Stock);
public record CreateProductDto(string Name, decimal Price, int Stock);
