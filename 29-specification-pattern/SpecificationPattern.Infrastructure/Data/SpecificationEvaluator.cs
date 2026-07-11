using Microsoft.EntityFrameworkCore;
using SpecificationPattern.Application.Specifications;

namespace SpecificationPattern.Infrastructure.Data;

/// <summary>
/// Evaluator that converts our ISpecification into an EF Core executable IQueryable.
/// </summary>
public static class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery;

        // Apply criteria (Where clauses)
        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        // Apply eager loading (Includes)
        query = spec.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // Apply ordering
        if (spec.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }
        else if (spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        // Apply paging (must be done after filtering and ordering)
        if (spec.Skip.HasValue)
        {
            query = query.Skip(spec.Skip.Value);
        }

        if (spec.Take.HasValue)
        {
            query = query.Take(spec.Take.Value);
        }

        return query;
    }
}
