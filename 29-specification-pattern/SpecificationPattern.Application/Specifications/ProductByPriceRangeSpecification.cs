using SpecificationPattern.Application.Models;

namespace SpecificationPattern.Application.Specifications;

/// <summary>
/// Specification to filter products within a price range.
/// </summary>
public class ProductByPriceRangeSpecification : BaseSpecification<Product>
{
    public ProductByPriceRangeSpecification(decimal minPrice, decimal maxPrice) 
        : base(p => p.Price >= minPrice && p.Price <= maxPrice)
    {
    }
}
