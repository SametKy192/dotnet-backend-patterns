using System.Diagnostics;
using TemplateMethod.Api.Models;

namespace TemplateMethod.Api.Exporters;

/// <summary>
/// Abstract base class — the Template Method lives here.
///
/// <para>
/// <c>Export</c> is the <em>template method</em>: it defines the fixed skeleton
/// of the export algorithm (validate → open → write header → write rows → write footer → close).
/// Subclasses override the <em>hook</em> and <em>abstract</em> steps to supply
/// format-specific behaviour without changing the overall flow.
/// </para>
/// </summary>
public abstract class DataExporter
{
    // ── Template method (sealed — callers must use Export) ────────────────────

    /// <summary>
    /// Executes the fixed export pipeline and returns a result object.
    /// Override the protected hooks to customise each step.
    /// </summary>
    public ExportResult Export(IReadOnlyList<Product> products)
    {
        var sw = Stopwatch.StartNew();

        Validate(products);

        var sb = new System.Text.StringBuilder();
        Open(sb);
        WriteHeader(sb);

        foreach (var product in products)
            WriteRow(sb, product);

        WriteFooter(sb, products.Count);
        Close(sb);

        sw.Stop();

        return new ExportResult(
            Format:      FormatName,
            ContentType: ContentType,
            FileName:    $"products_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{FileExtension}",
            Content:     sb.ToString(),
            RowCount:    products.Count,
            ElapsedMs:   sw.ElapsedMilliseconds);
    }

    // ── Abstract steps — must be implemented by every concrete exporter ────────

    /// <summary>Human-readable format name (e.g. "CSV", "JSON").</summary>
    protected abstract string FormatName    { get; }

    /// <summary>MIME content type for the HTTP response.</summary>
    protected abstract string ContentType   { get; }

    /// <summary>File extension without the leading dot.</summary>
    protected abstract string FileExtension { get; }

    /// <summary>Writes the format-specific header (column names, opening tag, etc.).</summary>
    protected abstract void WriteHeader(System.Text.StringBuilder sb);

    /// <summary>Writes a single product row in the target format.</summary>
    protected abstract void WriteRow(System.Text.StringBuilder sb, Product product);

    // ── Hooks — optional overrides with sensible defaults ─────────────────────

    /// <summary>
    /// Called before anything is written.
    /// Override to write an opening document wrapper (e.g. XML declaration).
    /// Default: no-op.
    /// </summary>
    protected virtual void Open(System.Text.StringBuilder sb)  { }

    /// <summary>
    /// Called after all rows are written.
    /// Override to write a closing tag, summary row, etc.
    /// Default: no-op.
    /// </summary>
    protected virtual void WriteFooter(System.Text.StringBuilder sb, int rowCount) { }

    /// <summary>
    /// Called last.
    /// Override to append a closing document wrapper.
    /// Default: no-op.
    /// </summary>
    protected virtual void Close(System.Text.StringBuilder sb)  { }

    /// <summary>
    /// Validates the input list before processing starts.
    /// Default: throws <see cref="ArgumentException"/> if the list is null or empty.
    /// </summary>
    protected virtual void Validate(IReadOnlyList<Product> products)
    {
        if (products is null || products.Count == 0)
            throw new ArgumentException("Product list must not be null or empty.", nameof(products));
    }
}
