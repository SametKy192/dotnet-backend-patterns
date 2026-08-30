using EventSourcing.Api.Infrastructure;
using EventSourcing.Api.Projections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryEventStore>();
builder.Services.AddSingleton<AccountProjection>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Event Sourcing API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Sourcing API v1"));
}

app.UseAuthorization();
app.MapControllers();
app.Run();
