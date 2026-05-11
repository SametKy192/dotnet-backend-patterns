using BackgroundService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// EmailQueueService — singleton olmalı
// Hem controller hem worker aynı instance'ı kullanır
builder.Services.AddSingleton<EmailQueueService>();

// EmailWorker — background service olarak kaydet, uygulama başlayınca çalışır
builder.Services.AddHostedService<EmailWorker>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();