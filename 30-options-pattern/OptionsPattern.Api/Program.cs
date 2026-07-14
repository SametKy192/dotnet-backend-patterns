using OptionsPattern.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Options with DataAnnotations validation and ValidateOnStart.
// If configurations in appsettings.json are invalid, the app will fail to start.
builder.Services.AddOptions<SmtpSettings>()
    .Bind(builder.Configuration.GetSection(SmtpSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Listen to options change using IOptionsMonitor to log modifications.
var monitor = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<SmtpSettings>>();
monitor.OnChange((settings, name) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("SMTP Settings changed dynamically: Server={Server}, Port={Port}", settings.Server, settings.Port);
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
