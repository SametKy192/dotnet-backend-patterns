using FastEndpoints;
using FastEndpointsDemo.Api.Data;

namespace FastEndpointsDemo.Api.Endpoints;

public record GetProductRequest(int Id);
public record GetProductResponse(int Id, string Name, decimal Price);

public class GetProductEndpoint : Endpoint<GetProductRequest, GetProductResponse>
{
    private readonly ProductStore _store;

    public GetProductEndpoint(ProductStore store)
    {
        _store = store;
    }

    public override void Configure()
    {
        Get("/api/products/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
    {
        var product = _store.GetById(req.Id);
        if (product == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new GetProductResponse(product.Id, product.Name, product.Price);
        await Send.ResponseAsync(response, 200, ct);
    }
}
