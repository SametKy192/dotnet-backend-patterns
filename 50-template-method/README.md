# 50 — Template Method Pattern

Demonstrates the **Template Method** design pattern in ASP.NET Core — defines the skeleton of an algorithm in an abstract base class, deferring specific steps to subclasses without altering the structure.

## Overview

- **Base Class:** `DataExporter` (defines `Export(products)` pipeline: Validate -> Open -> Header -> Rows -> Footer -> Close)
- **Concrete Subclasses:** `CsvExporter`, `JsonExporter`, `XmlExporter`, `MarkdownExporter`
- **Controller:** `ExportController` with per-format and dynamic routes.
