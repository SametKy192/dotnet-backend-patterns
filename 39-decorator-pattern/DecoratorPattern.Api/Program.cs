using DecoratorPattern.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Decorator pattern composition:
// Resolution order: LoggingDecorator -> CachingDecorator -> ProductService
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<IProductService>(sp =>
{
    var inner = sp.GetRequiredService<ProductService>();
    var cache = sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
    var logger = sp.GetRequiredService<ILogger<LoggingProductServiceDecorator>>();

    // Layer 1: Base service
    IProductService service = inner;
    // Layer 2: Wrap with caching
    service = new CachingProductServiceDecorator(service, cache);
    // Layer 3: Wrap with logging (outermost)
    service = new LoggingProductServiceDecorator(service, logger);

    return service;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
