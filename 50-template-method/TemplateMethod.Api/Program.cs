using TemplateMethod.Api.Exporters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register each concrete exporter as a singleton.
// All share the same abstract DataExporter pipeline; only their hook overrides differ.
// Exporters are thread-safe and stateless, making singleton lifetime ideal.
builder.Services.AddSingleton<CsvExporter>();
builder.Services.AddSingleton<JsonExporter>();
builder.Services.AddSingleton<XmlExporter>();
builder.Services.AddSingleton<MarkdownExporter>();

var app = builder.Build();

app.MapControllers();

app.Run();

public partial class Program { }
