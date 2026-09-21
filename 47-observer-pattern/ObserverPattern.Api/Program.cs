using ObserverPattern.Api.Observers;
using ObserverPattern.Api.Subject;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register the subject as a singleton so all controllers share the same market state
builder.Services.AddSingleton<IStockMarket, StockMarket>();

// Register each concrete observer as singleton under both the concrete type
// and the IStockObserver interface so the controller can enumerate all of them
builder.Services.AddSingleton<EmailAlertObserver>();
builder.Services.AddSingleton<SmsAlertObserver>();
builder.Services.AddSingleton<DashboardObserver>();
builder.Services.AddSingleton<AuditLogObserver>();

// Expose the same singleton instances under IStockObserver for IEnumerable<IStockObserver> injection
builder.Services.AddSingleton<IStockObserver>(sp => sp.GetRequiredService<EmailAlertObserver>());
builder.Services.AddSingleton<IStockObserver>(sp => sp.GetRequiredService<SmsAlertObserver>());
builder.Services.AddSingleton<IStockObserver>(sp => sp.GetRequiredService<DashboardObserver>());
builder.Services.AddSingleton<IStockObserver>(sp => sp.GetRequiredService<AuditLogObserver>());

var app = builder.Build();

app.MapControllers();

app.Run();

// Make Program partial so it is visible to the test project
public partial class Program { }
