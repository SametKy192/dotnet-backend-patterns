using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StronglyTypedIds.Api.Data;
using StronglyTypedIds.Api.Domain;
using StronglyTypedIds.Api.Domain.Entities;

namespace StronglyTypedIds.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Customers.AnyAsync() || await _context.Products.AnyAsync())
        {
            return BadRequest(new { Message = "Database is already seeded." });
        }

        var customer = new Customer(CustomerId.New(), "Samet Kaya");
        var product = new Product(ProductId.New(), "Mechanical Keyboard", 89.99m);

        _context.Customers.Add(customer);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Seed completed successfully.",
            CustomerId = customer.Id.ToString(),
            ProductId = product.Id.ToString()
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var customerId = new CustomerId(request.CustomerId);
        var productId = new ProductId(request.ProductId);

        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null) return NotFound(new { Error = "Customer not found." });

        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound(new { Error = "Product not found." });

        try
        {
            // Compile-time Safety Check:
            // The constructor expects: Order(OrderId id, CustomerId customerId, ProductId productId, int quantity, decimal unitPrice)
            // If you attempt to write:
            // var order = new Order(OrderId.New(), productId, customerId, request.Quantity, product.Price);
            // It will FAIL to compile! This prevents critical business bugs where developer swaps ID parameters.
            
            var order = new Order(OrderId.New(), customerId, productId, request.Quantity, product.Price);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                OrderId = order.Id.ToString(),
                CustomerId = order.CustomerId.ToString(),
                ProductId = order.ProductId.ToString(),
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders.ToListAsync();
        var response = orders.Select(o => new
        {
            OrderId = o.Id.ToString(),
            CustomerId = o.CustomerId.ToString(),
            ProductId = o.ProductId.ToString(),
            Quantity = o.Quantity,
            TotalPrice = o.TotalPrice
        });

        return Ok(response);
    }
}

public record CreateOrderRequest(Guid CustomerId, Guid ProductId, int Quantity);
