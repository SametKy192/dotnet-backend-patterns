using ChainOfResponsibility.Api.Handlers;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// Register each handler as a transient service
builder.Services.AddTransient<SpamFilterHandler>();
builder.Services.AddTransient<AuthenticationHandler>();
builder.Services.AddTransient<AuthorizationHandler>();
builder.Services.AddTransient<RateLimitHandler>();
builder.Services.AddTransient<LoggingHandler>();
builder.Services.AddTransient<SupportHandler>();

// Build the handler pipeline and register it as the entry point
builder.Services.AddSingleton<ITicketHandler>(sp =>
{
    var spam    = sp.GetRequiredService<SpamFilterHandler>();
    var auth    = sp.GetRequiredService<AuthenticationHandler>();
    var authz   = sp.GetRequiredService<AuthorizationHandler>();
    var rate    = sp.GetRequiredService<RateLimitHandler>();
    var log     = sp.GetRequiredService<LoggingHandler>();
    var support = sp.GetRequiredService<SupportHandler>();

    // Chain: SpamFilter → Authentication → Authorization → RateLimit → Logging → Support
    spam.SetNext(auth)
        .SetNext(authz)
        .SetNext(rate)
        .SetNext(log)
        .SetNext(support);

    return spam;
});

// ── App ────────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

