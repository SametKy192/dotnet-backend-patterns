# 39 — Vertical Slice Architecture

A clean implementation of the Vertical Slice Architecture in ASP.NET Core. 

Unlike traditional layered architectures (like Clean Architecture or Onion Architecture) that organize code horizontally (e.g., Controllers, Services, Repositories, Domain), Vertical Slice Architecture organizes code around **features** or **business capabilities** (slices).

## Key Concepts

- **Feature-centric organization**: Every slice contains everything it needs to fulfill a specific request (API endpoint, MediatR command/query, validation rules, DB operations).
- **High Cohesion, Low Coupling**: Changes to a specific feature only affect that feature's files.
- **Maintainability**: New developers can understand the entire feature flow by looking at a single folder/file.

## Project Structure

```
39-vertical-slice-architecture/
├── VerticalSlice.Api/
│   ├── Features/
│   │   └── Products/
│   │       ├── CreateProduct.cs   <-- Contains Endpoint, Command, Validator, Handler
│   │       └── GetProducts.cs     <-- Contains Endpoint, Query, Handler
│   └── Program.cs
└── README.md
```
