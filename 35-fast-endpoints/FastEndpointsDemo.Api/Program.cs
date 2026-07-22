using FastEndpoints;
using FastEndpoints.Swagger;
using FastEndpointsDemo.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Register Product Store as singleton
builder.Services.AddSingleton<ProductStore>();

// Register FastEndpoints, explicitly providing the Api assembly for scanning
builder.Services.AddFastEndpoints(o =>
{
    o.Assemblies = new[] { typeof(Program).Assembly };
});

// Configure FastEndpoints Swagger Integration
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "FastEndpoints Demo API";
        s.Version = "v1";
    };
});

var app = builder.Build();

// Configure the HTTP pipeline.
app.UseFastEndpoints();

// Configure Swagger
app.UseSwaggerGen();

app.Run();
