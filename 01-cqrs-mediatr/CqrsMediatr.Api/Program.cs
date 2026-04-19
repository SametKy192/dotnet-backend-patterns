using CqrsMediatr.Application.Common.Behaviors;
using CqrsMediatr.Application.Products.Commands.CreateProduct;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// MediatR — Application katmanındaki tüm handler'ları otomatik bulur
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

// ValidationBehavior'ı pipeline'a ekle — her request'ten önce çalışır
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));

// FluentValidation — Application katmanındaki tüm validator'ları otomatik bulur
builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();