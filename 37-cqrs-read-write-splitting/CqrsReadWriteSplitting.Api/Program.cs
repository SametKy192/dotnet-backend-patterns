using CqrsReadWriteSplitting.Api.Data;
using CqrsReadWriteSplitting.Api.Domain.Queries;
using CqrsReadWriteSplitting.Api.Domain.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Create and open SQLite connection to share between EF Core and Dapper
var connection = new SqliteConnection("Filename=:memory:");
connection.Open();

// Register the connection as a singleton so it can be resolved by query services
builder.Services.AddSingleton(connection);

// Register EF Core AppDbContext for commands/writes
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connection));

// Register repositories and queries
builder.Services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
builder.Services.AddScoped<IProductQueries, ProductQueries>();

builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure EF Core schema is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CQRS Read-Write Splitting API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
