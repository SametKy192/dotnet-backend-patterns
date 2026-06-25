using DistributedLock.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace DistributedLock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    /// <summary>
    /// Tüm ürünleri ve stok miktarlarını getir
    /// GET /api/stock
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_stockService.GetProducts());
    }

    /// <summary>
    /// Stok düşür — distributed lock ile güvenli.
    /// Aynı anda 10 istek gelse bile race condition olmaz.
    /// POST /api/stock/{id}/decrement
    /// </summary>
    [HttpPost("{id}/decrement")]
    public async Task<IActionResult> Decrement(int id, [FromBody] DecrementRequest request)
    {
        var success = await _stockService.DecrementStockAsync(id, request.Quantity);

        if (!success)
            return Conflict(new { Message = "Stok düşürülemedi — lock alınamadı veya stok yetersiz" });

        return Ok(new { Message = "Stok düşürüldü", ProductId = id, Quantity = request.Quantity });
    }

    /// <summary>
    /// Lock OLMADAN stok düşür — race condition simülasyonu.
    /// Aynı anda birden fazla istek gelince yanlış sonuç üretir.
    /// POST /api/stock/{id}/decrement-unsafe
    /// </summary>
    [HttpPost("{id}/decrement-unsafe")]
    public async Task<IActionResult> DecrementUnsafe(int id, [FromBody] DecrementRequest request)
    {
        var success = await _stockService.DecrementStockUnsafeAsync(id, request.Quantity);

        if (!success)
            return Conflict(new { Message = "Stok düşürülemedi" });

        return Ok(new { Message = "Stok düşürüldü (UNSAFE)", ProductId = id, Quantity = request.Quantity });
    }

    /// <summary>
    /// Race condition testi — aynı anda 5 istek gönderir
    /// POST /api/stock/{id}/race-test
    /// </summary>
    [HttpPost("{id}/race-test")]
    public async Task<IActionResult> RaceTest(int id)
    {
        var tasks = Enumerable.Range(1, 5)
            .Select(i => _stockService.DecrementStockAsync(id, 1))
            .ToList();

        var results = await Task.WhenAll(tasks);

        return Ok(new
        {
            TotalRequests = 5,
            Successful = results.Count(r => r),
            Failed = results.Count(r => !r),
            Products = _stockService.GetProducts()
        });
    }
}

public record DecrementRequest(int Quantity);