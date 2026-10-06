using FluentAssertions;
using TemplateMethod.Api.Exporters;
using TemplateMethod.Api.Models;

namespace TemplateMethod.Tests;

/// <summary>
/// 25 unit tests covering all four exporter subclasses and the DataExporter
/// base-class validation hook.
/// </summary>
public class TemplateMethodTests
{
    // ── Seed data ─────────────────────────────────────────────────────────────

    private static readonly List<Product> OneProduct =
    [
        new(Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"), "Laptop", "Electronics", 999.99m, 10)
    ];

    private static readonly List<Product> TwoProducts =
    [
        new(Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"), "Laptop",       "Electronics", 999.99m, 10),
        new(Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002"), "Standing Desk","Furniture",   449.00m,  5)
    ];

    // ── Validation (base class hook) ──────────────────────────────────────────

    [Fact]
    public void Export_EmptyList_ThrowsArgumentException()
    {
        var exporter = new CsvExporter();
        exporter.Invoking(e => e.Export([]))
            .Should().Throw<ArgumentException>()
            .WithMessage("*null or empty*");
    }

    [Fact]
    public void Export_ReturnsCorrectRowCount()
    {
        var result = new CsvExporter().Export(TwoProducts);
        result.RowCount.Should().Be(2);
    }

    [Fact]
    public void Export_ReturnsNonNegativeElapsedMs()
    {
        var result = new CsvExporter().Export(OneProduct);
        result.ElapsedMs.Should().BeGreaterThanOrEqualTo(0);
    }

    // ── CsvExporter ───────────────────────────────────────────────────────────

    [Fact]
    public void Csv_FormatName_IsCsv()
        => new CsvExporter().Export(OneProduct).Format.Should().Be("CSV");

    [Fact]
    public void Csv_ContentType_IsTextCsv()
        => new CsvExporter().Export(OneProduct).ContentType.Should().Be("text/csv");

    [Fact]
    public void Csv_FileName_EndsWithCsvExtension()
        => new CsvExporter().Export(OneProduct).FileName.Should().EndWith(".csv");

    [Fact]
    public void Csv_Content_ContainsHeaderRow()
    {
        var result = new CsvExporter().Export(OneProduct);
        result.Content.Should().Contain("Id,Name,Category,Price,Stock");
    }

    [Fact]
    public void Csv_Content_ContainsProductData()
    {
        var result = new CsvExporter().Export(OneProduct);
        result.Content.Should().Contain("Laptop").And.Contain("999.99");
    }

    [Fact]
    public void Csv_Content_EscapesCommasInName()
    {
        var product = new Product(Guid.NewGuid(), "Laptop, Pro", "Electronics", 100m, 1);
        var result  = new CsvExporter().Export([product]);
        result.Content.Should().Contain("\"Laptop, Pro\"");
    }

    // ── JsonExporter ──────────────────────────────────────────────────────────

    [Fact]
    public void Json_FormatName_IsJson()
        => new JsonExporter().Export(OneProduct).Format.Should().Be("JSON");

    [Fact]
    public void Json_ContentType_IsApplicationJson()
        => new JsonExporter().Export(OneProduct).ContentType.Should().Be("application/json");

    [Fact]
    public void Json_FileName_EndsWithJsonExtension()
        => new JsonExporter().Export(OneProduct).FileName.Should().EndWith(".json");

    [Fact]
    public void Json_Content_StartsWithOpenBracket()
    {
        var result = new JsonExporter().Export(OneProduct);
        result.Content.TrimStart().Should().StartWith("[");
    }

    [Fact]
    public void Json_Content_EndsWithCloseBracket()
    {
        var result = new JsonExporter().Export(OneProduct);
        result.Content.TrimEnd().Should().EndWith("]");
    }

    [Fact]
    public void Json_Content_ContainsProductName()
    {
        var result = new JsonExporter().Export(OneProduct);
        result.Content.Should().Contain("\"name\"").And.Contain("Laptop");
    }

    // ── XmlExporter ───────────────────────────────────────────────────────────

    [Fact]
    public void Xml_FormatName_IsXml()
        => new XmlExporter().Export(OneProduct).Format.Should().Be("XML");

    [Fact]
    public void Xml_ContentType_IsApplicationXml()
        => new XmlExporter().Export(OneProduct).ContentType.Should().Be("application/xml");

    [Fact]
    public void Xml_Content_StartsWithXmlDeclaration()
    {
        var result = new XmlExporter().Export(OneProduct);
        result.Content.Should().StartWith("<?xml");
    }

    [Fact]
    public void Xml_Content_ContainsProductElement()
    {
        var result = new XmlExporter().Export(OneProduct);
        result.Content.Should().Contain("<product ").And.Contain("</product>");
    }

    [Fact]
    public void Xml_Content_ContainsTotalComment()
    {
        var result = new XmlExporter().Export(TwoProducts);
        result.Content.Should().Contain("total: 2 products");
    }

    [Fact]
    public void Xml_Content_EscapesSpecialChars()
    {
        var product = new Product(Guid.NewGuid(), "Desk & Chair", "Furniture", 500m, 3);
        var result  = new XmlExporter().Export([product]);
        result.Content.Should().Contain("&amp;");
    }

    // ── MarkdownExporter ──────────────────────────────────────────────────────

    [Fact]
    public void Markdown_FormatName_IsMarkdown()
        => new MarkdownExporter().Export(OneProduct).Format.Should().Be("Markdown");

    [Fact]
    public void Markdown_ContentType_IsTextMarkdown()
        => new MarkdownExporter().Export(OneProduct).ContentType.Should().Be("text/markdown");

    [Fact]
    public void Markdown_Content_ContainsTableHeader()
    {
        var result = new MarkdownExporter().Export(OneProduct);
        result.Content.Should().Contain("| Id |").And.Contain("| Name |");
    }

    [Fact]
    public void Markdown_Content_ContainsSummaryFooter()
    {
        var result = new MarkdownExporter().Export(TwoProducts);
        result.Content.Should().Contain("Total: 2 product(s)");
    }
}
