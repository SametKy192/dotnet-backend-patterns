using FluentValidation;
using MediatR;
using PipelineBehavior.Api.Behaviors;
using PipelineBehavior.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// ── MediatR ─────────────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// ── Pipeline Behaviors (order matters: outer → inner) ───────────────────────
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

// ── FluentValidation ─────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ── Exception handling middleware ─────────────────────────────────────────────
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Translate ValidationException → 400 Problem Details
app.Use(async (ctx, next) =>
{
    try
    {
        await next(ctx);
    }
    catch (ValidationException ex)
    {
        ctx.Response.StatusCode = 400;
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        await ctx.Response.WriteAsJsonAsync(new
        {
            type    = "https://tools.ietf.org/html/rfc7807",
            title   = "Validation failed",
            status  = 400,
            errors
        });
    }
});

// ── Endpoints ─────────────────────────────────────────────────────────────────
app.MapProductEndpoints();

app.Run();
