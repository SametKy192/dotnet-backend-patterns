using Microsoft.EntityFrameworkCore;
using OutboxPattern.Infrastructure.Persistence;
using OutboxPattern.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// In-memory veritabanı — gerçek projede PostgreSQL olur
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("OutboxDb"));

// OutboxProcessor background service olarak kaydet — uygulama başlayınca çalışır
builder.Services.AddHostedService<OutboxProcessor>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();