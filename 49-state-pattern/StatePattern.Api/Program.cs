using StatePattern.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<OrderRepository>();

var app = builder.Build();

app.MapControllers();

app.Run();

// Partial class so the test project can reference Program
public partial class Program { }
