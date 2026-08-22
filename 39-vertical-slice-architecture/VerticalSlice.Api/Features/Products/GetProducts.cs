using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace VerticalSlice.Api.Features.Products;

public static class GetProducts
{
    public record Query() : IRequest<List<CreateProduct.Response>>;

    public class Handler : IRequestHandler<Query, List<CreateProduct.Response>>
    {
        public Task<List<CreateProduct.Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            return Task.FromResult(CreateProduct.Handler.Products);
        }
    }

    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/products", async (ISender sender) =>
        {
            var result = await sender.Send(new Query());
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .WithTags("Products");
    }
}
