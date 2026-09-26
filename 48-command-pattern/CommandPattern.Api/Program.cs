using CommandPattern.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// AccountRepository is a singleton — it holds all accounts and the shared
// BankCommandInvoker (with its undo/redo stacks) for the lifetime of the app
builder.Services.AddSingleton<AccountRepository>();

var app = builder.Build();

app.MapControllers();

app.Run();

// Partial class so the test project can reference Program
public partial class Program { }
