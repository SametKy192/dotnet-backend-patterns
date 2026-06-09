
# =============================================
# 15-unit-testing/README.md
# =============================================

# 15 — Unit Testing

A .NET 8 unit testing implementation using xUnit, Moq, and FluentAssertions.

## What You'll Learn
- xUnit test structure (Fact, Theory)
- Moq for mocking dependencies
- FluentAssertions for readable assertions
- AAA pattern (Arrange, Act, Assert)
- Testing business logic in isolation

## Key Concepts

### Fact vs Theory
```csharp
// Fact — single test case
[Fact]
public async Task GetById_WhenExists_ReturnsProduct() { }

// Theory — multiple test cases with same logic
[Theory]
[InlineData(-1)]
[InlineData(0)]
public async Task Create_WithInvalidPrice_Throws(decimal price) { }
```

### Moq
```csharp
// Setup — define what mock returns
_repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

// Verify — assert mock was called
_repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
```

### FluentAssertions
```csharp
result.Should().HaveCount(2);
result.Should().Contain(p => p.Name == "Laptop");
await action.Should().ThrowAsync<KeyNotFoundException>();
```

## Project Structure

```
UnitTesting.Application/
├── Interfaces/
│   └── IProductRepository.cs   ← Mocked in tests
├── Models/
│   └── Product.cs
└── Services/
    └── ProductService.cs       ← System Under Test

UnitTesting.Tests/
└── ProductServiceTests.cs      ← All test cases
```

## Run Tests

```bash
cd UnitTesting.Tests
dotnet test
```

## Test Coverage

| Method | Test Cases |
|--------|-----------|
| GetAllAsync | Returns list, returns empty |
| GetByIdAsync | Found, not found |
| CreateAsync | Valid data, invalid price (3 cases), empty name (3 cases) |
| DeleteAsync | Exists, not found |

## Packages Used

| Package | Purpose |
|---------|---------|
| xUnit | Test framework |
| Moq | Mocking |
| FluentAssertions | Readable assertions |

---