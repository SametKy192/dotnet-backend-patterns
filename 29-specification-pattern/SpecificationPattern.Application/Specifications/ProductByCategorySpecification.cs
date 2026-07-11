using SpecificationPattern.Application.Models;

namespace SpecificationPattern.Application.Specifications;

/// <summary>
/// Specification to filter products by category.
/// </summary>
public class ProductByCategorySpecification : BaseSpecification<Product>
{
    public ProductByCategorySpecification(string category) 
        : base(p => p.Category == category)
    {
    }
}
