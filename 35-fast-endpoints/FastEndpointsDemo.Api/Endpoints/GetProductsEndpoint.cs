using FastEndpoints;
using FastEndpointsDemo.Api.Data;

namespace FastEndpointsDemo.Api.Endpoints;

public record GetProductsResponse(int Id, string Name, decimal Price);

public class GetProductsEndpoint : EndpointWithoutRequest<IEnumerable<GetProductsResponse>>
{
    private readonly ProductStore _store;

    public GetProductsEndpoint(ProductStore store)
    {
        _store = store;
    }

    public override void Configure()
    {
        Get("/api/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var products = _store.GetAll()
            .Select(p => new GetProductsResponse(p.Id, p.Name, p.Price));

        await Send.ResponseAsync(products, 200, ct);
    }
}
