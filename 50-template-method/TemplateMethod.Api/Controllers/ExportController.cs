using Microsoft.AspNetCore.Mvc;
using TemplateMethod.Api.Exporters;
using TemplateMethod.Api.Models;

namespace TemplateMethod.Api.Controllers;

/// <summary>
/// REST façade for the Template Method pattern demo.
/// Clients pick a format; the controller delegates to the matching exporter subclass.
/// The export pipeline (validate → open → header → rows → footer → close) always
/// runs in the same order — only the format-specific steps differ.
/// </summary>
[ApiController]
[Route("api/export")]
public class ExportController(
    CsvExporter      csv,
    JsonExporter     json,
    XmlExporter      xml,
    MarkdownExporter markdown) : ControllerBase
{
    // ── Seed data ─────────────────────────────────────────────────────────────

    private static readonly IReadOnlyList<Product> SeedProducts =
    [
        new(Guid.Parse("11111111-0000-0000-0000-000000000001"), "Laptop Pro 15",   "Electronics", 1299.99m, 42),
        new(Guid.Parse("22222222-0000-0000-0000-000000000002"), "Wireless Mouse",  "Electronics",   29.99m, 210),
        new(Guid.Parse("33333333-0000-0000-0000-000000000003"), "Standing Desk",   "Furniture",    449.00m,  18),
        new(Guid.Parse("44444444-0000-0000-0000-000000000004"), "Ergonomic Chair", "Furniture",    389.50m,  27),
        new(Guid.Parse("55555555-0000-0000-0000-000000000005"), "USB-C Hub 7-in-1","Electronics",   49.95m, 135),
    ];

    // ── Endpoints ─────────────────────────────────────────────────────────────

    /// <summary>Export products as CSV.</summary>
    [HttpGet("csv")]
    public IActionResult ExportCsv()      => ExportWith(csv);

    /// <summary>Export products as JSON.</summary>
    [HttpGet("json")]
    public IActionResult ExportJson()     => ExportWith(json);

    /// <summary>Export products as XML.</summary>
    [HttpGet("xml")]
    public IActionResult ExportXml()      => ExportWith(xml);

    /// <summary>Export products as a Markdown table.</summary>
    [HttpGet("markdown")]
    public IActionResult ExportMarkdown() => ExportWith(markdown);

    /// <summary>
    /// Export using a format name supplied in the route — case-insensitive.
    /// Supported: csv, json, xml, markdown.
    /// </summary>
    [HttpGet("{format}")]
    public IActionResult ExportByFormat(string format)
    {
        DataExporter? exporter = format.ToLowerInvariant() switch
        {
            "csv"      => csv,
            "json"     => json,
            "xml"      => xml,
            "markdown" => markdown,
            _          => null
        };

        if (exporter is null)
            return BadRequest(new { error = $"Unsupported format '{format}'. Valid values: csv, json, xml, markdown." });

        return ExportWith(exporter);
    }

    /// <summary>Lists all available export formats.</summary>
    [HttpGet]
    public IActionResult GetFormats()
        => Ok(new { formats = new[] { "csv", "json", "xml", "markdown" } });

    // ── Helper ────────────────────────────────────────────────────────────────

    private IActionResult ExportWith(DataExporter exporter)
    {
        try
        {
            var result = exporter.Export(SeedProducts);
            return Ok(new
            {
                format      = result.Format,
                fileName    = result.FileName,
                rowCount    = result.RowCount,
                elapsedMs   = result.ElapsedMs,
                content     = result.Content
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
