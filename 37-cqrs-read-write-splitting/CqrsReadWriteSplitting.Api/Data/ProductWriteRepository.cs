using CqrsReadWriteSplitting.Api.Domain.Entities;
using CqrsReadWriteSplitting.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CqrsReadWriteSplitting.Api.Data;

public class ProductWriteRepository : IProductWriteRepository
{
    private readonly AppDbContext _context;

    public ProductWriteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product, CancellationToken ct)
    {
        await _context.Products.AddAsync(product, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Products.FindAsync(new object[] { id }, ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(ct);
    }
}
