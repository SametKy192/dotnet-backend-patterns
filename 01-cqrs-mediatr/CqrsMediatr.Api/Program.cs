using CqrsMediatr.Application.Products.Commands.CreateProduct;

var builder = WebApplication.CreateBuilder(args);

// Controller'ları servise ekle
builder.Services.AddControllers();

// MediatR'ı kaydet — Application katmanındaki tüm handler'ları otomatik bulur
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

// Swagger
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