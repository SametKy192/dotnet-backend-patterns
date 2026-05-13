using Grpc.Core;

namespace GrpcDemo.Server.Services;

/// <summary>
/// gRPC servis implementasyonu.
/// Proto dosyasından otomatik üretilen base class'tan türetilir.
/// </summary>
public class ProductGrpcService : ProductService.ProductServiceBase
{
    private readonly ILogger<ProductGrpcService> _logger;

    // Örnek veri — gerçek projede DB'den gelir
    private static readonly List<(int Id, string Name, double Price)> Products = new()
    {
        (1, "Laptop", 999.99),
        (2, "Mouse", 29.99),
        (3, "Keyboard", 79.99)
    };

    public ProductGrpcService(ILogger<ProductGrpcService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tek ürün getir — unary call
    /// </summary>
    public override Task<ProductResponse> GetProduct(
        GetProductRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("GetProduct: {Id}", request.Id);

        var product = Products.FirstOrDefault(p => p.Id == request.Id);

        if (product == default)
        {
            // gRPC'de hata fırlatma — HTTP 404 gibi
            throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Ürün bulunamadı: {request.Id}"));
        }

        return Task.FromResult(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        });
    }

    /// <summary>
    /// Tüm ürünleri stream olarak gönder — server streaming.
    /// Ürünler teker teker gelir, istemci bağlantıyı açık tutar.
    /// </summary>
    public override async Task GetAllProducts(
        GetAllProductsRequest request,
        IServerStreamWriter<ProductResponse> responseStream,
        ServerCallContext context)
    {
        foreach (var product in Products)
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;

            await responseStream.WriteAsync(new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            });

            _logger.LogInformation("Stream: {Name} gönderildi", product.Name);
            await Task.Delay(300);
        }
    }
}