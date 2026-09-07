using ResultPattern.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddSingleton<IProductService, ProductService>();

// ── App ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
