using FastEndpoints;
using FluentValidation;

namespace FastEndpointsDemo.Api.Endpoints;

public class CreateProductValidator : Validator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MinimumLength(3).WithMessage("Product name must be at least 3 characters long.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}
