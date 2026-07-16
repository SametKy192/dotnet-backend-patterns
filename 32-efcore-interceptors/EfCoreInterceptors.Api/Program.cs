using Microsoft.EntityFrameworkCore;
using EfCoreInterceptors.Api.Data;
using EfCoreInterceptors.Api.Data.Interceptors;
using EfCoreInterceptors.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register HttpContext Accessor and Current User Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Register the SaveChanges Interceptor
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

// Register DbContext with In-Memory Database and the Interceptor
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var auditInterceptor = sp.GetRequiredService<AuditSaveChangesInterceptor>();
    options.UseInMemoryDatabase("InterceptorsDb")
           .AddInterceptors(auditInterceptor);
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    
    // Seed some initial products if database is empty
    if (!db.Products.Any())
    {
        // Notice we don't set CreatedAtUtc or CreatedBy manually!
        // The interceptor will handle it, but here it runs under "System" since there's no HTTP context.
        db.Products.AddRange(
            new() { Name = "Wireless Mouse", Price = 29.99m },
            new() { Name = "Mechanical Keyboard", Price = 89.99m }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EF Core Interceptors API v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
