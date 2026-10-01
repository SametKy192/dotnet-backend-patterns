namespace TemplateMethod.Api.Models;

/// <summary>The result returned by any exporter.</summary>
public record ExportResult(
    string Format,
    string ContentType,
    string FileName,
    string Content,
    int    RowCount,
    long   ElapsedMs);
