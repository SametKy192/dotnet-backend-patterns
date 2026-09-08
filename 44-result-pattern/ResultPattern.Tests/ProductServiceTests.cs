using ResultPattern.Api.Models;
using ResultPattern.Api.Services;

namespace ResultPattern.Tests;

public sealed class ProductServiceTests
{
    private readonly ProductService _sut = new();

    // ── GetAll ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_WhenNoProducts_ShouldReturnEmptyList()
    {
        var result = await _sut.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedProduct()
    {
        var request = new CreateProductRequest("Widget", "A nice widget", 9.99m, 100);

        var result = await _sut.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Widget", result.Value.Name);
        Assert.Equal(9.99m, result.Value.Price);
        Assert.Equal(100, result.Value.StockQuantity);
    }

    [Fact]
    public async Task Create_WithEmptyName_ShouldReturnValidationError()
    {
        var request = new CreateProductRequest("", "desc", 10m, 5);

        var result = await _sut.CreateAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Validation.Name", result.Error.Code);
    }

    [Fact]
    public async Task Create_WithNegativePrice_ShouldReturnValidationError()
    {
        var request = new CreateProductRequest("Widget", "desc", -1m, 5);

        var result = await _sut.CreateAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Validation.Price", result.Error.Code);
    }

    [Fact]
    public async Task Create_WithNegativeStock_ShouldReturnValidationError()
    {
        var request = new CreateProductRequest("Widget", "desc", 5m, -1);

        var result = await _sut.CreateAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Validation.StockQuantity", result.Error.Code);
    }

    [Fact]
    public async Task Create_WithDuplicateName_ShouldReturnConflictError()
    {
        var request = new CreateProductRequest("Gadget", "desc", 20m, 10);
        await _sut.CreateAsync(request);

        var duplicate = await _sut.CreateAsync(request);

        Assert.True(duplicate.IsFailure);
        Assert.Equal("Product.Conflict", duplicate.Error.Code);
    }

    // ── GetById ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnProduct()
    {
        var created = (await _sut.CreateAsync(new("Camera", "desc", 299m, 5))).Value;

        var result = await _sut.GetByIdAsync(created.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(created.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFoundError()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_WhenExists_ShouldReturnUpdatedProduct()
    {
        var created = (await _sut.CreateAsync(new("OldName", "desc", 5m, 1))).Value;
        var req = new UpdateProductRequest("NewName", "updated desc", 10m, 2);

        var result = await _sut.UpdateAsync(created.Id, req);

        Assert.True(result.IsSuccess);
        Assert.Equal("NewName", result.Value.Name);
        Assert.Equal(10m, result.Value.Price);
    }

    [Fact]
    public async Task Update_WhenNotFound_ShouldReturnNotFoundError()
    {
        var req = new UpdateProductRequest("X", "desc", 1m, 1);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), req);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_WhenExists_ShouldReturnOk()
    {
        var created = (await _sut.CreateAsync(new("ToDelete", "desc", 1m, 1))).Value;

        var result = await _sut.DeleteAsync(created.Id);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFoundError()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    // ── UpdateStock ───────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStock_WithPositiveDelta_ShouldIncreaseStock()
    {
        var created = (await _sut.CreateAsync(new("Stock Item", "desc", 5m, 10))).Value;

        var result = await _sut.UpdateStockAsync(created.Id, 5);

        Assert.True(result.IsSuccess);
        Assert.Equal(15, result.Value.StockQuantity);
    }

    [Fact]
    public async Task UpdateStock_ThatWouldMakeStockNegative_ShouldReturnInsufficientStockError()
    {
        var created = (await _sut.CreateAsync(new("Low Stock", "desc", 5m, 3))).Value;

        var result = await _sut.UpdateStockAsync(created.Id, -10);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.InsufficientStock", result.Error.Code);
    }
}
