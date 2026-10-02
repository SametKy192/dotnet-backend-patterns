using TemplateMethod.Api.Models;

namespace TemplateMethod.Api.Exporters;

/// <summary>
/// Exports products as comma-separated values (CSV).
/// Header → one row per product.
/// </summary>
public class CsvExporter : DataExporter
{
    protected override string FormatName    => "CSV";
    protected override string ContentType   => "text/csv";
    protected override string FileExtension => "csv";

    protected override void WriteHeader(System.Text.StringBuilder sb)
        => sb.AppendLine("Id,Name,Category,Price,Stock");

    protected override void WriteRow(System.Text.StringBuilder sb, Product p)
        => sb.AppendLine($"{p.Id},{EscapeCsv(p.Name)},{EscapeCsv(p.Category)},{p.Price:F2},{p.Stock}");

    // Wrap values that contain commas or quotes in double-quotes
    private static string EscapeCsv(string value)
        => value.Contains(',') || value.Contains('"')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
