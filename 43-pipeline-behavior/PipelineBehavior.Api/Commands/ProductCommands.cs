using MediatR;
using PipelineBehavior.Api.Models;

namespace PipelineBehavior.Api.Commands;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock) : IRequest<ProductDto>;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock) : IRequest<ProductDto?>;

public sealed record DeleteProductCommand(Guid Id) : IRequest<bool>;
