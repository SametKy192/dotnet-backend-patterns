using Microsoft.EntityFrameworkCore;
using MinimalApi.Api.Data;
using MinimalApi.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Minimal API — controller yerine endpoint kullanır
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("MinimalApiDb"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Endpoint'leri kaydet — extension method ile organize
app.MapProductEndpoints();

app.Run();