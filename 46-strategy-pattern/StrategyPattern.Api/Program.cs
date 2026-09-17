using StrategyPattern.Api.Services;
using StrategyPattern.Api.Strategies;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// Register all shipping strategies — each implements IShippingStrategy
builder.Services.AddSingleton<IShippingStrategy, StandardShippingStrategy>();
builder.Services.AddSingleton<IShippingStrategy, ExpressShippingStrategy>();
builder.Services.AddSingleton<IShippingStrategy, OvernightShippingStrategy>();
builder.Services.AddSingleton<IShippingStrategy, FreeShippingStrategy>();
builder.Services.AddSingleton<IShippingStrategy, InternationalShippingStrategy>();

// Factory resolves IEnumerable<IShippingStrategy> automatically
builder.Services.AddSingleton<ShippingStrategyFactory>();

// ── App ────────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
