using System.Data;
using Dapper;
using DapperDemo.Infrastructure.Data;
using DapperDemo.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// SQLite bağlantısı — Dapper direkt IDbConnection kullanır
// Gerçek projede: new SqlConnection(connectionString) veya NpgsqlConnection
builder.Services.AddScoped<IDbConnection>(_ =>
    new SqliteConnection("Data Source=dapper-demo.db"));

builder.Services.AddScoped<ProductRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Veritabanını başlat
using (var scope = app.Services.CreateScope())
{
    var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
    connection.Open();
    var initializer = new DbInitializer(connection);
    initializer.Initialize();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();