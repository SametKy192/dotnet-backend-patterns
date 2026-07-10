using System.Linq.Expressions;

namespace SpecificationPattern.Application.Specifications;

/// <summary>
/// Specification interface — sorgu kriterini kapsüller.
/// Her specification bir iş kuralını temsil eder.
/// Birleştirilebilir: And, Or, Not operatörleri ile.
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// Filtreleme ifadesi — EF Core'a gönderilir, SQL'e dönüşür
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Include'lar — eager loading için
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Sıralama
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Sayfalama
    /// </summary>
    int? Take { get; }
    int? Skip { get; }
}