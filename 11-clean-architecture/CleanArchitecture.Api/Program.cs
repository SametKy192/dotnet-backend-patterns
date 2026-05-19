using CleanArchitecture.Application.Products;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Infrastructure — DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("CleanArchDb"));

// Dependency Inversion — interface'e karşı concrete class kayıt
// Domain IProductRepository'yi tanır, Infrastructure ProductRepository'yi inject eder
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Application servisi
builder.Services.AddScoped<ProductService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();