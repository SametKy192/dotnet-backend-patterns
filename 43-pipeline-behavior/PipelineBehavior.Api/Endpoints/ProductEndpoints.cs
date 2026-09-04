using MediatR;
using PipelineBehavior.Api.Commands;
using PipelineBehavior.Api.Queries;

namespace PipelineBehavior.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var products = await mediator.Send(new GetAllProductsQuery());
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .WithSummary("Get all products (cached 30s)");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var product = await mediator.Send(new GetProductByIdQuery(id));
            return product is null ? Results.NotFound() : Results.Ok(product);
        })
        .WithName("GetProductById")
        .WithSummary("Get product by ID (cached 30s)");

        group.MapPost("/", async (CreateProductCommand command, IMediator mediator) =>
        {
            var product = await mediator.Send(command);
            return Results.Created($"/api/products/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .WithSummary("Create a new product (validated)");

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductCommand command, IMediator mediator) =>
        {
            var updated = await mediator.Send(command with { Id = id });
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        })
        .WithName("UpdateProduct")
        .WithSummary("Update an existing product (validated)");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var deleted = await mediator.Send(new DeleteProductCommand(id));
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithSummary("Delete a product");
    }
}
