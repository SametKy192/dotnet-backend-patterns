using FastEndpoints;
using FastEndpointsDemo.Api.Data;
using FastEndpointsDemo.Api.Entities;

namespace FastEndpointsDemo.Api.Endpoints;

public record CreateProductRequest(string Name, decimal Price);
public record CreateProductResponse(int Id, string Name, decimal Price);

public class CreateProductEndpoint : Endpoint<CreateProductRequest, CreateProductResponse>
{
    private readonly ProductStore _store;

    public CreateProductEndpoint(ProductStore store)
    {
        _store = store;
    }

    public override void Configure()
    {
        Post("/api/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var product = new Product
        {
            Name = req.Name,
            Price = req.Price
        };

        _store.Add(product);

        var response = new CreateProductResponse(product.Id, product.Name, product.Price);
        await Send.ResponseAsync(response, 201, ct);
    }
}
