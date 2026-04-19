using FluentValidation;

namespace CqrsMediatr.Application.Products.Commands.CreateProduct;

/// <summary>
/// CreateProductCommand için validation kuralları.
/// FluentValidation kütüphanesi kullanılıyor.
/// Bu sınıf otomatik olarak ValidationBehavior tarafından çağrılır.
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        // Name boş olamaz
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı boş olamaz.")
            .MaximumLength(100).WithMessage("Ürün adı 100 karakterden uzun olamaz.");

        // Price 0'dan büyük olmalı
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");
    }
}