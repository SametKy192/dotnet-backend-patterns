using System.Text;
using System.Text.Json;
using TemplateMethod.Api.Models;

namespace TemplateMethod.Api.Exporters;

/// <summary>
/// Exports products as a pretty-printed JSON array.
/// Open → array bracket, WriteRow → JSON object per product, Close → closing bracket.
/// </summary>
public class JsonExporter : DataExporter
{
    private bool _firstRow = true;

    protected override string FormatName    => "JSON";
    protected override string ContentType   => "application/json";
    protected override string FileExtension => "json";

    protected override void Open(StringBuilder sb)
    {
        _firstRow = true;
        sb.AppendLine("[");
    }

    // Header is not applicable for JSON — skip
    protected override void WriteHeader(StringBuilder sb) { }

    protected override void WriteRow(StringBuilder sb, Product p)
    {
        if (!_firstRow) sb.AppendLine(",");
        _firstRow = false;

        var obj = new
        {
            id       = p.Id,
            name     = p.Name,
            category = p.Category,
            price    = p.Price,
            stock    = p.Stock
        };

        sb.Append("  " + JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false }));
    }

    protected override void Close(StringBuilder sb)
    {
        sb.AppendLine();
        sb.Append("]");
    }
}
