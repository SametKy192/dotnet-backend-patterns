namespace TemplateMethod.Api.Models;

/// <summary>The result returned by any exporter.</summary>
public record ExportResult(
    string Format,
    string ContentType,
    string FileName,
    string Content,
    int    RowCount,
    long   ElapsedMs)
{
    /// <summary>Size of the exported content in bytes (UTF-8).</summary>
    public int ContentLength => System.Text.Encoding.UTF8.GetByteCount(Content);
}

