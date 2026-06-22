using MassTransit;
using SagaStateMachine.Saga;
using SagaStateMachine.Saga.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    // Consumer'ları kaydet
    x.AddConsumer<StockConsumer>();
    x.AddConsumer<ReleaseStockConsumer>();
    x.AddConsumer<PaymentConsumer>();

    // Saga state machine kaydet — in-memory storage
    x.AddSagaStateMachine<OrderSagaStateMachine, OrderSagaState>()
        .InMemoryRepository();

    x.UsingInMemory((context, cfg) =>
    {
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