# 35 — REPR Pattern with FastEndpoints

A .NET 10 implementation of the **REPR (Request-Endpoint-Response) Pattern** using **FastEndpoints**, presenting a clean, feature-based alternative to traditional MVC controllers and Minimal APIs.

## What You'll Learn
- What the REPR design pattern is and why it improves Vertical Slice architecture
- How to define endpoints, request contracts, and response contracts in a single file/folder (features)
- How to implement request validation using `FluentValidation` integrated into FastEndpoints
- Setting up Swagger documentation natively with FastEndpoints
- Comparing MVC Controllers, Minimal APIs, and FastEndpoints

---

## What is the REPR Pattern?

Traditional ASP.NET Core MVC controllers group multiple actions (endpoints) for a single resource in one class:
- `ProductsController` contains `GetProduct`, `CreateProduct`, `UpdateProduct`, `DeleteProduct`, etc.

As application complexity grows, these controller classes become bloated, making it difficult to maintain and violating the Single Responsibility Principle.

**REPR (Request-Endpoint-Response)** pattern divides this so that each endpoint is its own class, focusing on exactly one action:
- **Request**: DTO representing the incoming HTTP request.
- **Endpoint**: Single class containing the business logic handler for that request.
- **Response**: DTO representing the outgoing HTTP response.

This structure aligns perfectly with **Vertical Slice Architecture**, keeping all files related to a feature in one place.

---

## MVC vs. Minimal APIs vs. FastEndpoints

| Aspect | MVC Controllers | Minimal APIs | FastEndpoints |
|---|---|---|---|
| **Class Size** | Large & Bloated (Multi-action) | N/A (Defined in Program.cs/Extensions) | Small & Focused (Single-action class) |
| **Testability** | Medium (Requires mocking many dependencies) | Hard (Testing lambda handlers inline) | High (Standard classes with typed request/response) |
| **Performance** | Slower (Reflective routing) | Fast (Direct invocation) | Fast (Uses source generator compilation) |
| **Validation** | Data Annotations | Manual / Filter pipelines | Automatic FluentValidation integration |

---

## Implementation Details

### 1. The Endpoint
We inherit from `Endpoint<TRequest, TResponse>` to define the contract and logic:
```csharp
public class GetProductEndpoint : Endpoint<GetProductRequest, GetProductResponse>
{
    public override void Configure()
    {
        Get("/api/products/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
    {
        var product = _store.GetById(req.Id);
        if (product == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(new(product.Id, product.Name, product.Price), cancellation: ct);
    }
}
```

### 2. Automatic FluentValidation
We inherit from `Validator<TRequest>` to define validation rules. FastEndpoints automatically executes the validator if it matches the request contract of the endpoint:
```csharp
public class CreateProductValidator : Validator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

---

## Running the Project

```bash
cd FastEndpointsDemo.Api
dotnet run
```

- Swagger UI is available at `http://localhost:5034/swagger`.
- Use the provided `requests.http` file to execute API requests.
- Observe that sending an invalid request automatically returns a beautifully structured RFC 7807 Bad Request response with validation details.
