using Microsoft.EntityFrameworkCore;
using SoftDeleteEfCore.Api.Data;
using SoftDeleteEfCore.Api.Data.Interceptors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the SoftDeleteInterceptor
builder.Services.AddScoped<SoftDeleteInterceptor>();

// Register DbContext with In-Memory Database and the Interceptor
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var softDeleteInterceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
    options.UseInMemoryDatabase("SoftDeleteDb")
           .AddInterceptors(softDeleteInterceptor);
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
    
    // Seed some initial users if database is empty
    if (!db.Users.IgnoreQueryFilters().Any())
    {
        db.Users.AddRange(
            new() { Name = "Alice Smith", Email = "alice@example.com", IsDeleted = false },
            new() { Name = "Bob Johnson", Email = "bob@example.com", IsDeleted = false },
            new() { 
                Name = "Charlie Brown", 
                Email = "charlie@example.com", 
                IsDeleted = true, 
                DeletedAtUtc = DateTime.UtcNow.AddDays(-1) 
            }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Soft Delete EF Core API v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
