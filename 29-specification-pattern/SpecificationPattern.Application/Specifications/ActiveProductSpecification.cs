using SpecificationPattern.Application.Models;

namespace SpecificationPattern.Application.Specifications;

/// <summary>
/// Specification to filter only active products.
/// </summary>
public class ActiveProductSpecification : BaseSpecification<Product>
{
    public ActiveProductSpecification() : base(p => p.IsActive)
    {
    }
}
