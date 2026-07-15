using Microsoft.AspNetCore.Mvc;
using UnitOfWorkPattern.Api.Data;
using UnitOfWorkPattern.Api.Entities;

namespace UnitOfWorkPattern.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public OrdersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return Ok(products);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request.Quantity <= 0)
        {
            return BadRequest("Quantity must be greater than zero.");
        }

        // Start Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. Get Product
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return NotFound($"Product with ID {request.ProductId} not found.");
            }

            // 2. Check Stock
            if (product.Stock < request.Quantity)
            {
                // Throwing exception to demonstrate rollback flow
                throw new InvalidOperationException(
                    $"Insufficient stock. Requested: {request.Quantity}, Available: {product.Stock}");
            }

            // 3. Update Stock
            product.Stock -= request.Quantity;
            _unitOfWork.Products.Update(product);

            // 4. Create Order
            var order = new Order
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                TotalAmount = product.Price * request.Quantity
            };
            await _unitOfWork.Orders.AddAsync(order);

            // 5. Save Changes
            await _unitOfWork.SaveChangesAsync();

            // 6. Commit Transaction
            await _unitOfWork.CommitTransactionAsync();

            return Ok(order);
        }
        catch (Exception ex)
        {
            // If anything goes wrong, rollback all operations (order creation and stock modification)
            await _unitOfWork.RollbackTransactionAsync();
            return BadRequest(new { Error = ex.Message });
        }
    }
}

public record CreateOrderRequest(int ProductId, int Quantity);
