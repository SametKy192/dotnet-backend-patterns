using System.Text;
using TemplateMethod.Api.Models;

namespace TemplateMethod.Api.Exporters;

/// <summary>
/// Exports products as a Markdown table — useful for reports, wikis and README previews.
/// Header → column row + separator, WriteRow → one table row per product,
/// WriteFooter → summary line.
/// </summary>
public class MarkdownExporter : DataExporter
{
    protected override string FormatName    => "Markdown";
    protected override string ContentType   => "text/markdown";
    protected override string FileExtension => "md";

    protected override void WriteHeader(StringBuilder sb)
    {
        sb.AppendLine("| Id | Name | Category | Price | Stock |");
        sb.AppendLine("|----|------|----------|------:|------:|");
    }

    protected override void WriteRow(StringBuilder sb, Product p)
        => sb.AppendLine($"| {p.Id} | {p.Name} | {p.Category} | {p.Price:F2} | {p.Stock} |");

    protected override void WriteFooter(StringBuilder sb, int rowCount)
    {
        sb.AppendLine();
        sb.AppendLine($"*Total: {rowCount} product(s) — exported {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC*");
    }
}
