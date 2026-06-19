using YarpGateway.Gateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// YARP — appsettings.json'daki ReverseProxy konfigürasyonunu okur
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Gateway logging middleware
app.UseMiddleware<GatewayLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// YARP reverse proxy endpoint'lerini map et
app.MapReverseProxy();

app.Run();