using Microsoft.EntityFrameworkCore;
using MinimalApi.Api.Data;
using MinimalApi.Api.Models;

namespace MinimalApi.Api.Endpoints;

/// <summary>
/// Ürün endpoint'leri — Minimal API ile tanımlanır.
/// Controller sınıfı yok, doğrudan lambda ile endpoint tanımlanır.
/// Daha az boilerplate, daha hızlı.
/// </summary>
public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        // Endpoint grubu — ortak prefix ve tag
        var group = app.MapGroup("/api/products")
            .WithTags("Products")
            .WithOpenApi();

        // Tüm ürünleri getirir
        group.MapGet("/", async (AppDbContext db) =>
        {
            var products = await db.Products.ToListAsync();
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .WithSummary("Get all products");

        // Id ile ürün getirir
        group.MapGet("/{id}", async (int id, AppDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        })
        .WithName("GetProductById")
        .WithSummary("Get product by id");

        // Yeni ürün oluşturur
        group.MapPost("/", async (CreateProductRequest request, AppDbContext db) =>
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            return Results.Created($"/api/products/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .WithSummary("Create a product");

        // Ürün günceller
        group.MapPut("/{id}", async (int id, CreateProductRequest request, AppDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            product.Name = request.Name;
            product.Price = request.Price;
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .WithSummary("Update a product");

        // Ürün siler
        group.MapDelete("/{id}", async (int id, AppDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .WithSummary("Delete a product");
    }
}

/// <summary>
/// Ürün oluşturma/güncelleme isteği
/// </summary>
public record CreateProductRequest(string Name, decimal Price);