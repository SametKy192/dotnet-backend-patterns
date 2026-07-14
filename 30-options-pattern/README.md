# 30 — Options Pattern

A .NET 10 implementation of the **Options Pattern** demonstrating strongly-typed configuration binding, DataAnnotations-based validation, startup validation, and the differences between `IOptions<T>`, `IOptionsSnapshot<T>`, and `IOptionsMonitor<T>`.

## What You'll Learn
- How to implement the Options Pattern in ASP.NET Core
- How to validate configurations on startup using `ValidateDataAnnotations()` and `ValidateOnStart()`
- The difference in lifetimes and behavior between `IOptions`, `IOptionsSnapshot`, and `IOptionsMonitor`
- How to listen to configuration changes dynamically using `IOptionsMonitor.OnChange`

## Lifetimes & Differences

| Option Type | Registration Lifetime | Dynamic Updates | Description |
|---|---|---|---|
| **`IOptions<T>`** | Singleton | No | Evaluated once at registration. It will **not** pick up changes in `appsettings.json` without restarting the app. |
| **`IOptionsSnapshot<T>`** | Scoped | Yes | Evaluated once per request. It **reloads** configurations automatically if `appsettings.json` is modified. Cannot be injected into Singleton services. |
| **`IOptionsMonitor<T>`** | Singleton | Yes (Real-time) | Singleton wrapper that retrieves current values dynamically at any time. Supports change notifications (`OnChange`). Can be safely injected into Singletons. |

## Startup Validation

To prevent starting the application with invalid configuration values, we configure options validation:
```csharp
builder.Services.AddOptions<SmtpSettings>()
    .Bind(builder.Configuration.GetSection(SmtpSettings.SectionName))
    .ValidateDataAnnotations() // Validates properties using attributes like [Required] and [Range]
    .ValidateOnStart();       // Forces the application to validate and fail during startup rather than runtime
```

If we set an invalid port in `appsettings.json` (e.g., `-1`), the application will immediately crash on boot with an options validation exception:
```
OptionsValidationException: One or more uncommon errors occurred. (Port must be between 1 and 65535.)
```

## Running the Project

```bash
cd OptionsPattern.Api
dotnet run
```

Access Swagger UI at `http://localhost:5030/swagger`.
Uset the `requests.http` to query settings. Modify `appsettings.json` while the API is running to witness `IOptionsSnapshot` and `IOptionsMonitor` update dynamically!
