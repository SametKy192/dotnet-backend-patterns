using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace VerticalSlice.Api.Features.Products;

public static class CreateProduct
{
    // 1. Request & Response Models (DTOs)
    public record Request(string Name, decimal Price);
    public record Response(int Id, string Name, decimal Price);

    // 2. MediatR Command
    public record Command(string Name, decimal Price) : IRequest<Response>;

    // 3. Validation Rules (FluentValidation)
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }

    // 4. MediatR Handler
    public class Handler : IRequestHandler<Command, Response>
    {
        // In-memory db simulation for simplicity
        public static readonly List<Response> Products = new();

        public Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = new Response(Products.Count + 1, request.Name, request.Price);
            Products.Add(product);
            return Task.FromResult(product);
        }
    }

    // 5. Minimal API Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/products", async (Request request, ISender sender) =>
        {
            var command = new Command(request.Name, request.Price);
            
            // Validation step
            var validator = new Validator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var response = await sender.Send(command);
            return Results.Created($"/api/products/{response.Id}", response);
        })
        .WithName("CreateProduct")
        .WithTags("Products");
    }
}
