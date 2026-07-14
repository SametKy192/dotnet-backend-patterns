using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OptionsPattern.Api.Models;

namespace OptionsPattern.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly IOptions<SmtpSettings> _options;
    private readonly IOptionsSnapshot<SmtpSettings> _optionsSnapshot;
    private readonly IOptionsMonitor<SmtpSettings> _optionsMonitor;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        IOptions<SmtpSettings> options,
        IOptionsSnapshot<SmtpSettings> optionsSnapshot,
        IOptionsMonitor<SmtpSettings> optionsMonitor,
        ILogger<SettingsController> logger)
    {
        _options = options;
        _optionsSnapshot = optionsSnapshot;
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetSettings()
    {
        // 1. IOptions: Evaluated once at registration (singleton). Does not pick up config changes until restart.
        var optionsVal = _options.Value;

        // 2. IOptionsSnapshot: Evaluated once per request (scoped). Useful when you want options to reload on config changes.
        var snapshotVal = _optionsSnapshot.Value;

        // 3. IOptionsMonitor: Retrieves current values at any time (singleton). Real-time changes, includes OnChange event.
        var monitorVal = _optionsMonitor.CurrentValue;

        return Ok(new
        {
            IOptions = new
            {
                optionsVal.Server,
                optionsVal.Port,
                optionsVal.SenderEmail,
                optionsVal.SenderName,
                optionsVal.EnableSsl,
                LifetimeInfo = "Singleton - Stays the same until application restart"
            },
            IOptionsSnapshot = new
            {
                snapshotVal.Server,
                snapshotVal.Port,
                snapshotVal.SenderEmail,
                snapshotVal.SenderName,
                snapshotVal.EnableSsl,
                LifetimeInfo = "Scoped - Reloads on every HTTP request if appsettings.json changes"
            },
            IOptionsMonitor = new
            {
                monitorVal.Server,
                monitorVal.Port,
                monitorVal.SenderEmail,
                monitorVal.SenderName,
                monitorVal.EnableSsl,
                LifetimeInfo = "Singleton (Dynamic) - Retrieves current configuration value in real-time"
            }
        });
    }
}
