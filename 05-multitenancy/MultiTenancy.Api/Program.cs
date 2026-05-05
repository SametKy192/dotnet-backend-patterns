using Microsoft.EntityFrameworkCore;
using MultiTenancy.Infrastructure.Persistence;
using MultiTenancy.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// HttpContextAccessor — TenantService için gerekli
builder.Services.AddHttpContextAccessor();

// TenantService — her request'te header'dan tenant okur
builder.Services.AddScoped<TenantService>();

// DbContext — TenantService inject edilir
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("MultiTenancyDb"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();