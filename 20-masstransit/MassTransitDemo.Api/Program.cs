using MassTransit;
using MassTransitDemo.Consumers.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// MassTransit + RabbitMQ yapılandırması
builder.Services.AddMassTransit(x =>
{
    // Consumer'ları kaydet
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<OrderShippedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // RabbitMQ bağlantısı
        // Docker ile başlatmak için: docker run -p 5672:5672 -p 15672:15672 rabbitmq:management
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        // Consumer endpoint'lerini otomatik yapılandır
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();