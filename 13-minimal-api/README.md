# 13 — Minimal API + OpenAPI

A .NET 8 Minimal API implementation with OpenAPI documentation and endpoint grouping.

## What You'll Learn
- Minimal API vs Controller-based API
- Endpoint grouping with MapGroup
- OpenAPI metadata with WithSummary, WithName
- Results helper methods

## Minimal API vs Controller

| | Controller | Minimal API |
|--|-----------|-------------|
| Boilerplate | More | Less |
| Organization | Class-based | Function-based |
| Performance | Slightly slower | Slightly faster |
| Best for | Large APIs | Small/medium APIs |

## How It Works