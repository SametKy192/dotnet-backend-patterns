using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTesting.Application.Interfaces;
using UnitTesting.Application.Models;
using UnitTesting.Application.Services;

namespace UnitTesting.Tests;

/// <summary>
/// ProductService unit testleri.
/// Moq ile repository mock'lanır — gerçek DB'ye gidilmez.
/// FluentAssertions ile okunabilir assertion'lar yazılır.
/// </summary>
public class ProductServiceTests
{
    // Mock nesneler — her testte sıfırlanır
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _sut; // System Under Test

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        // Mock'ları inject et — gerçek implementasyon yerine mock kullanılır
        _sut = new ProductService(_repositoryMock.Object, _loggerMock.Object);
    }

    // ===== GetAllAsync =====

    [Fact]
    public async Task GetAllAsync_WhenProductsExist_ReturnsAllProducts()
    {
        // Arrange — test verisini hazırla
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 999.99m },
            new() { Id = 2, Name = "Mouse", Price = 29.99m }
        };

        // Repository mock'unu ayarla — GetAllAsync çağrılınca bu listeyi döndür
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        // Act — test edilecek metodu çalıştır
        var result = await _sut.GetAllAsync();

        // Assert — sonucu doğrula (FluentAssertions)
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Laptop");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProducts_ReturnsEmptyList()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    // ===== GetByIdAsync =====

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Laptop", Price = 999.99m };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange — null döndür, ürün yok
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act & Assert — exception fırlatılmalı
        await _sut.Invoking(s => s.GetByIdAsync(99))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    // ===== CreateAsync =====

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Laptop", Price = 999.99m };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync(product);

        // Act
        var result = await _sut.CreateAsync("Laptop", 999.99m);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Laptop");

        // Repository'nin AddAsync'i bir kez çağrıldığını doğrula
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task CreateAsync_WithInvalidPrice_ThrowsArgumentException(decimal price)
    {
        // Theory — birden fazla değerle aynı testi çalıştır
        await _sut.Invoking(s => s.CreateAsync("Laptop", price))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*0'dan büyük*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateAsync_WithEmptyName_ThrowsArgumentException(string? name)
    {
        await _sut.Invoking(s => s.CreateAsync(name!, 999.99m))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*boş olamaz*");
    }

    // ===== DeleteAsync =====

    [Fact]
    public async Task DeleteAsync_WhenProductExists_CallsRepositoryDelete()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Laptop", Price = 999.99m };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteAsync(1);

        // Assert — DeleteAsync'in bir kez çağrıldığını doğrula
        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act & Assert
        await _sut.Invoking(s => s.DeleteAsync(99))
            .Should().ThrowAsync<KeyNotFoundException>();

        // DeleteAsync hiç çağrılmamalı
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}