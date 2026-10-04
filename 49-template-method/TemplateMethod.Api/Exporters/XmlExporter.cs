using System.Globalization;
using System.Text;
using TemplateMethod.Api.Models;

namespace TemplateMethod.Api.Exporters;

/// <summary>
/// Exports products as an XML document.
/// Open → XML declaration + root element, WriteRow → &lt;product&gt; element,
/// WriteFooter → count attribute, Close → closing root tag.
/// </summary>
public class XmlExporter : DataExporter
{
    protected override string FormatName    => "XML";
    protected override string ContentType   => "application/xml";
    protected override string FileExtension => "xml";

    protected override void Open(StringBuilder sb)
    {
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<products>");
    }

    protected override void WriteHeader(StringBuilder sb) { /* no column header in XML */ }

    protected override void WriteRow(StringBuilder sb, Product p)
    {
        sb.AppendLine($"  <product id=\"{p.Id}\">");
        sb.AppendLine($"    <name>{Escape(p.Name)}</name>");
        sb.AppendLine($"    <category>{Escape(p.Category)}</category>");
        sb.AppendLine($"    <price>{p.Price.ToString("F2", CultureInfo.InvariantCulture)}</price>");
        sb.AppendLine($"    <stock>{p.Stock}</stock>");
        sb.AppendLine($"  </product>");
    }

    protected override void WriteFooter(StringBuilder sb, int rowCount)
        => sb.AppendLine($"  <!-- total: {rowCount} products -->");

    protected override void Close(StringBuilder sb)
        => sb.Append("</products>");

    private static string Escape(string value)
        => value
            .Replace("&",  "&amp;")
            .Replace("<",  "&lt;")
            .Replace(">",  "&gt;")
            .Replace("\"", "&quot;");
}
