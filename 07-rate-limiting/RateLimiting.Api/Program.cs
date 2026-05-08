using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    // Limit aşılınca 429 Too Many Requests döndür
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Fixed Window — her 10 saniyede max 5 istek
    // Pencere dolunca tüm istekler reddedilir, sonraki pencerede sıfırlanır
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    // Sliding Window — pencere kayar, daha smooth
    // Son 10 saniyede max 5 istek, 2 segmente bölünür
    options.AddSlidingWindowLimiter("sliding", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
        opt.SegmentsPerWindow = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    // Token Bucket — bucket'ta token birikir, her istek token harcar
    // Burst trafiğe izin verir — bucket dolu ise birden fazla istek geçer
    options.AddTokenBucketLimiter("token", opt =>
    {
        opt.TokenLimit = 10;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
        opt.TokensPerPeriod = 2;
        opt.AutoReplenishment = true;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Rate limiter middleware — routing'den sonra olmalı
app.UseRateLimiter();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();