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
// Instead of: Assert.Equal(2, result.Count)
result.Should().HaveCount(2);
result.Should().Contain(p => p.Name == "Laptop");
await action.Should().ThrowAsync<KeyNotFoundException>();
```

## Project Structure