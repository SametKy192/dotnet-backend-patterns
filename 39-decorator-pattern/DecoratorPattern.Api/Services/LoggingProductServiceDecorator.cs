namespace DecoratorPattern.Api.Services;

/// <summary>
/// DECORATOR 2: Logging
/// Wraps IProductService and adds structured operation logging.
/// Stacked on top of the Caching decorator.
/// </summary>
public class LoggingProductServiceDecorator : IProductService
{
    private readonly IProductService _inner;
    private readonly ILogger<LoggingProductServiceDecorator> _logger;

    public LoggingProductServiceDecorator(IProductService inner, ILogger<LoggingProductServiceDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("[ProductService] GetByIdAsync called for ID={Id}", id);
        var result = await _inner.GetByIdAsync(id);
        _logger.LogInformation("[ProductService] GetByIdAsync result: {Found}", result != null ? "Found" : "Not Found");
        return result;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        _logger.LogInformation("[ProductService] GetAllAsync called");
        var result = await _inner.GetAllAsync();
        _logger.LogInformation("[ProductService] GetAllAsync returned {Count} products", result.Count());
        return result;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        _logger.LogInformation("[ProductService] CreateAsync called for product: {Name}", dto.Name);
        var result = await _inner.CreateAsync(dto);
        _logger.LogInformation("[ProductService] CreateAsync succeeded. New ID={Id}", result.Id);
        return result;
    }
}
