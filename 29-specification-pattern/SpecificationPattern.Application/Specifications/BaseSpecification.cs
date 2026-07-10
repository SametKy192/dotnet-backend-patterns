using System.Linq.Expressions;

namespace SpecificationPattern.Application.Specifications;

/// <summary>
/// Base specification — tüm specification'lar buradan türer.
/// And, Or, Not operatörleri ile birleştirilebilir.
/// </summary>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Include ekle — eager loading
    /// </summary>
    protected void AddInclude(Expression<Func<T, object>> include)
    {
        Includes.Add(include);
    }

    /// <summary>
    /// Sıralama ekle
    /// </summary>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderBy)
    {
        OrderBy = orderBy;
    }

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDesc)
    {
        OrderByDescending = orderByDesc;
    }

    /// <summary>
    /// Sayfalama ekle
    /// </summary>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    /// <summary>
    /// AND — her iki koşul da sağlanmalı
    /// </summary>
    public BaseSpecification<T> And(BaseSpecification<T> other)
    {
        var combined = new CombinedSpecification<T>(
            Combine(Criteria, other.Criteria, Expression.AndAlso));
        return combined;
    }

    /// <summary>
    /// OR — koşullardan biri sağlanmalı
    /// </summary>
    public BaseSpecification<T> Or(BaseSpecification<T> other)
    {
        var combined = new CombinedSpecification<T>(
            Combine(Criteria, other.Criteria, Expression.OrElse));
        return combined;
    }

    /// <summary>
    /// NOT — koşul sağlanmamalı
    /// </summary>
    public BaseSpecification<T> Not()
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.Not(Expression.Invoke(Criteria, param));
        var negated = Expression.Lambda<Func<T, bool>>(body, param);
        return new CombinedSpecification<T>(negated);
    }

    private static Expression<Func<T, bool>> Combine(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> merge)
    {
        var param = Expression.Parameter(typeof(T));
        var body = merge(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}

/// <summary>
/// Birleştirilmiş specification — And/Or/Not için kullanılır
/// </summary>
public class CombinedSpecification<T> : BaseSpecification<T>
{
    public CombinedSpecification(Expression<Func<T, bool>> criteria)
        : base(criteria)
    {
    }
}