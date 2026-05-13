# 10 — gRPC Demo

A .NET 8 gRPC implementation with unary and server streaming calls, consumed by a REST API client.

## What You'll Learn
- What gRPC is and how it differs from REST
- Protobuf service definition
- Unary and server streaming calls
- .NET gRPC client usage

## REST vs gRPC

| | REST | gRPC |
|--|------|------|
| Format | JSON | Protobuf (binary) |
| Speed | Normal | Much faster |
| Streaming | Difficult | Native |
| Use Case | Public API | Service-to-service |

## How It Works