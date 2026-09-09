# 44 — Result Pattern

Demonstrates the **Result Pattern** in ASP.NET Core — a functional approach to error handling that eliminates exception-driven control flow for *expected* failure cases.

## Concept

Instead of throwing exceptions (or returning `null`) when a domain operation fails, every service method returns a `Result<TValue>` (or `Result` for void operations). The caller **must** handle both the success and failure paths, making error handling explicit and type-safe.

```
Service Layer          →   Result<Product>
Controller Layer       →   Match() → IActionResult
```

## Project Structure

```
44-result-pattern/
├── ResultPattern.Api/
│   ├── Common/
│   │   ├── Error.cs              # Error record (code + description)
│   │   └── Result.cs             # Result<T> and non-generic Result
│   ├── Domain/
│   │   ├── Product.cs            # Product entity
│   │   └── ProductErrors.cs      # Strongly-typed domain errors
│   ├── Models/
│   │   └── ProductModels.cs      # Request / Response DTOs
│   ├── Services/
│   │   ├── IProductService.cs    # Service interface
│   │   └── ProductService.cs     # In-memory implementation
│   ├── Controllers/
│   │   └── ProductsController.cs # Thin controller using Match()
│   └── Program.cs
└── ResultPattern.Tests/
    ├── ResultTests.cs            # Result<T> & Result unit tests
    └── ProductServiceTests.cs    # ProductService unit tests (25 tests)
```

## Key Types

### `Error`
```csharp
public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static Error NotFound(string resource, object id) => ...
    public static Error Validation(string field, string message) => ...
    public static Error Conflict(string resource, string reason) => ...
}
```

### `Result<TValue>`
```csharp
public sealed class Result<TValue>
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
    public TValue Value { get; }
    public Error Error { get; }

    public static Result<TValue> Success(TValue value) => ...
    public static Result<TValue> Failure(Error error) => ...

    // Implicit conversions
    public static implicit operator Result<TValue>(TValue value) => ...
    public static implicit operator Result<TValue>(Error error) => ...

    // Railway-oriented programming
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) => ...
}
```

## Service Layer — No Exceptions

```csharp
public async Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request, ...)
{
    // Validation failure returns an Error, never throws
    if (string.IsNullOrWhiteSpace(request.Name))
        return ProductErrors.NameRequired; // implicit conversion to Result<T>

    // Conflict check
    if (_store.Values.Any(p => p.Name == request.Name))
        return ProductErrors.NameAlreadyExists(request.Name);

    var product = Product.Create(...);
    return MapToResponse(product); // implicit success
}
```

## Controller Layer — Match-Based Dispatch

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateProductRequest request, ...)
{
    var result = await _productService.CreateAsync(request);
    return result.Match<IActionResult>(
        product => CreatedAtAction(nameof(GetById), new { id = product.Id }, product),
        HandleError); // maps Error codes → HTTP status codes
}
```

## Error → HTTP Status Mapping

| Error Code Pattern       | HTTP Status                   |
|--------------------------|-------------------------------|
| `*.NotFound`             | 404 Not Found                 |
| `Validation.*`           | 400 Bad Request               |
| `*.Conflict`             | 409 Conflict                  |
| `Auth.Unauthorized`      | 401 Unauthorized              |
| `Product.InsufficientStock` | 422 Unprocessable Entity  |

## API Endpoints

| Method | Route                          | Description              |
|--------|--------------------------------|--------------------------|
| GET    | `/api/products`                | List all products         |
| GET    | `/api/products/{id}`           | Get product by ID         |
| POST   | `/api/products`                | Create product            |
| PUT    | `/api/products/{id}`           | Update product            |
| DELETE | `/api/products/{id}`           | Delete product            |
| PATCH  | `/api/products/{id}/stock`     | Adjust stock quantity     |

## Running the API

```bash
dotnet run --project ResultPattern.Api
```

## Running Tests

```bash
dotnet test
# Başarılı! - Başarısız: 0, Başarılı: 25, Atlanan: 0
```

## Benefits vs. Exception-Based Approach

| Concern | Exception-Based | Result Pattern |
|---------|----------------|----------------|
| Expected failures | `throw new NotFoundException()` | `return ProductErrors.NotFound(id)` |
| Caller awareness | Implicit (must know to catch) | Explicit (must handle both paths) |
| Control flow | Non-linear (exception unwinds) | Linear (return value propagation) |
| Performance | Expensive stack unwinding | Zero allocation on happy path |
| Testability | Requires `Assert.Throws<>` | Simple value assertions |
