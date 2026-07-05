using GlobalExceptionHandling.Api.Middleware;
using GlobalExceptionHandling.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Global exception handler — IExceptionHandler implementasyonu
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Problem Details — RFC 7807
builder.Services.AddProblemDetails();

builder.Services.AddScoped<ProductService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Exception handler middleware — en üstte olmalı
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();