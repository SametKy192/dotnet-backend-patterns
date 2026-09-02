using MediatR;
using PipelineBehavior.Api.Behaviors;
using PipelineBehavior.Api.Models;

namespace PipelineBehavior.Api.Queries;

public sealed record GetAllProductsQuery()
    : IRequest<IEnumerable<ProductDto>>, ICacheableRequest
{
    public string CacheKey => "all-products";
    public TimeSpan CacheDuration => TimeSpan.FromSeconds(30);
}

public sealed record GetProductByIdQuery(Guid Id)
    : IRequest<ProductDto?>, ICacheableRequest
{
    public string CacheKey => $"product-{Id}";
    public TimeSpan CacheDuration => TimeSpan.FromSeconds(30);
}
