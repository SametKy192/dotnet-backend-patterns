using EFCorePostgreSQL.Infrastructure.Persistence;
using EFCorePostgreSQL.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// In-memory DB — gerçek projede PostgreSQL bağlantısı:
// builder.Services.AddDbContext<AppDbContext>(opt =>
//     opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("EFCoreDb"));

builder.Services.AddScoped<ProductRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed data'yı uygula
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();