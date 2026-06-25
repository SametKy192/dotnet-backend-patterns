using DistributedLock.Infrastructure.Services;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Redis bağlantısı
// Docker ile başlat: docker run -p 6379:6379 redis
var redisConnection = await ConnectionMultiplexer.ConnectAsync(
    builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379");

// RedLock factory — distributed lock için
builder.Services.AddSingleton<RedLockNet.IDistributedLockFactory>(provider =>
{
    var endPoints = new List<RedLockMultiplexer>
    {
        new RedLockMultiplexer(redisConnection)
    };
    return RedLockFactory.Create(endPoints);
});

builder.Services.AddScoped<StockService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();