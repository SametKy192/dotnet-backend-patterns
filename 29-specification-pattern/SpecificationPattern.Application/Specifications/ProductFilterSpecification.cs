using SpecificationPattern.Application.Models;

namespace SpecificationPattern.Application.Specifications;

/// <summary>
/// A comprehensive specification that handles category, price range, active filter, sorting, and paging.
/// </summary>
public class ProductFilterSpecification : BaseSpecification<Product>
{
    public ProductFilterSpecification(string? category, decimal? minPrice, decimal? maxPrice, bool onlyActive = true)
        : base(p => (!onlyActive || p.IsActive) &&
                    (string.IsNullOrEmpty(category) || p.Category == category) &&
                    (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                    (!maxPrice.HasValue || p.Price <= maxPrice.Value))
    {
    }

    public void ApplySorting(string? sortBy)
    {
        switch (sortBy?.ToLower())
        {
            case "priceasc":
                ApplyOrderBy(p => p.Price);
                break;
            case "pricedesc":
                ApplyOrderByDescending(p => p.Price);
                break;
            case "nameasc":
                ApplyOrderBy(p => p.Name);
                break;
            case "namedesc":
                ApplyOrderByDescending(p => p.Name);
                break;
            default:
                ApplyOrderBy(p => p.Id);
                break;
        }
    }

    public void ApplyPagingOptions(int skip, int take)
    {
        ApplyPaging(skip, take);
    }
}
