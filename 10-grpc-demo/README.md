
# =============================================
# 10-grpc-demo/README.md
# =============================================

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

```
External world
    → REST → GrpcDemo.Client
                → gRPC (Protobuf) → GrpcDemo.Server
                                        → Returns data
```

## Project Structure

```
GrpcDemo.Server/
├── Protos/
│   └── product.proto           ← Service definition
└── Services/
    └── ProductGrpcService.cs   ← Implementation

GrpcDemo.Client/
└── Controllers/
    └── ProductsController.cs   ← REST → gRPC conversion
```

## Run

```bash
# Start server first
cd GrpcDemo.Server
dotnet run --urls=http://localhost:5200

# Then client (new terminal)
cd GrpcDemo.Client
dotnet run
```

## Endpoints (Client)

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products | All products (streaming) |
| GET | /api/products/{id} | Single product |

## Packages Used

| Package | Purpose |
|---------|---------|
| Grpc.Net.Client | gRPC client |
| Google.Protobuf | Protobuf serialization |
| Grpc.Tools | Code generation from .proto |

---