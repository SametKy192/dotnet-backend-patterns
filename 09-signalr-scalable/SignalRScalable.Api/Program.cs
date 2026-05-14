var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Redis bağlantısı appsettings'den al
var redisConnection = builder.Configuration.GetConnectionString("Redis");

if (!string.IsNullOrEmpty(redisConnection))
{
    // Redis varsa backplane ekle — birden fazla sunucu için gerekli
    builder.Services.AddSignalR()
        .AddStackExchangeRedis(redisConnection);
}
else
{
    // Redis yoksa sadece SignalR — tek sunucu için yeterli
    builder.Services.AddSignalR();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// SignalR hub endpoint'i — istemciler buraya WebSocket ile bağlanır
app.MapHub<SignalRScalable.Api.Hubs.NotificationHub>("/hubs/notification");

app.Run();