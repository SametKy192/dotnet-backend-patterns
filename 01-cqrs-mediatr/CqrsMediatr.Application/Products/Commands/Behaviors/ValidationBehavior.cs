using FluentValidation;
using MediatR;

namespace CqrsMediatr.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior — her request handler'a ulaşmadan önce buradan geçer.
/// Validator varsa çalıştırır, hata varsa exception fırlatır.
/// Validator yoksa direkt handler'a geçer.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// DI container'dan ilgili validator'lar inject edilir.
    /// Validator yoksa boş liste gelir.
    /// </summary>
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Her MediatR request'i buradan geçer.
    /// next() → bir sonraki pipeline adımını çağırır (handler dahil).
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Validator yoksa direkt geç
        if (!_validators.Any())
            return await next();

        // Tüm validator'ları çalıştır
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        // Hata varsa exception fırlat
        if (failures.Any())
            throw new ValidationException(failures);

        // Hata yoksa handler'a geç
        return await next();
    }
}