using IdempotencyPattern.Api.Middleware;
using IdempotencyPattern.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// In-memory cache — gerçek projede Redis kullanılır
builder.Services.AddMemoryCache();

// Idempotency servisi
builder.Services.AddScoped<IdempotencyService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Idempotency middleware — routing'den önce
app.UseMiddleware<IdempotencyMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();