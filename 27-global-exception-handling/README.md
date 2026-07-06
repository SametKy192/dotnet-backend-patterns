# 27 — Global Exception Handling + Problem Details

A .NET 8 implementation of centralized exception handling using IExceptionHandler and RFC 7807 Problem Details format.

## What You'll Learn
- IExceptionHandler for centralized error handling
- RFC 7807 Problem Details standard
- Custom exception hierarchy
- Controller-free try-catch
- Structured error responses

## Why Global Exception Handling?
Without Global Handler:
Every controller method needs try-catch → code duplication → inconsistent responses
With Global Handler:
Controllers have no try-catch → throw and forget → consistent RFC 7807 responses

## RFC 7807 Problem Details Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Kayıt Bulunamadı",
  "status": 404,
  "detail": "Product bulunamadı: 999",
  "instance": "/api/products/999",
  "errorCode": "NOT_FOUND"
}
```

## Exception Hierarchy
BaseException
├── NotFoundException (404)
├── ValidationException (400) ← includes field errors
├── ConflictException (409)
└── ForbiddenException (403)

## How It Works
Controller throws NotFoundException
→ No try-catch in controller
→ GlobalExceptionHandler.TryHandleAsync()
→ Maps to ProblemDetails
→ Returns RFC 7807 JSON response

## Controller — No try-catch needed

```csharp
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var product = _productService.GetById(id); // throws NotFoundException
    return Ok(product);
    // No try-catch! GlobalExceptionHandler handles it
}
```

## Project Structure
GlobalExceptionHandling.Application/
├── Exceptions/
│   ├── BaseException.cs
│   ├── NotFoundException.cs
│   ├── ValidationException.cs
│   ├── ConflictException.cs
│   └── ForbiddenException.cs
├── Models/
│   └── Product.cs
└── Services/
└── ProductService.cs
GlobalExceptionHandling.Api/
├── Middleware/
│   └── GlobalExceptionHandler.cs  ← IExceptionHandler
└── Controllers/
└── ProductsController.cs      ← No try-catch

## Run

```bash
cd GlobalExceptionHandling.Api
dotnet run
```

## Endpoints

| Method | URL | Happy Path | Exception |
|--------|-----|-----------|-----------|
| GET | /api/products | 200 | — |
| GET | /api/products/{id} | 200 | 404 NotFoundException |
| POST | /api/products | 201 | 400 ValidationException, 409 ConflictException |
| DELETE | /api/products/{id}?role= | 204 | 403 ForbiddenException, 404 NotFoundException |
| GET | /api/products/error | — | 500 Unexpected |

## Packages Used

| Package | Purpose |
|---------|---------|
| Built-in .NET 8 | IExceptionHandler, ProblemDetails |