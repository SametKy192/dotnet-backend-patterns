

# =============================================
# 16-docker/README.md
# =============================================

# 16 — Docker + docker-compose

A .NET 8 application containerized with Docker and orchestrated with docker-compose alongside PostgreSQL and Redis.

## What You'll Learn
- Multi-stage Dockerfile
- docker-compose with multiple services
- Environment variables in containers
- Named volumes for data persistence
- Container networking

## Multi-stage Dockerfile

```dockerfile
# Stage 1: Build (SDK image — larger)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN dotnet publish -o /app/publish

# Stage 2: Runtime (smaller image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
COPY --from=build /app/publish .
```

Why multi-stage? Final image only contains runtime, not SDK. Much smaller.

## Services

| Service | Image | Port |
|---------|-------|------|
| api | Custom (.NET 8) | 8080 |
| postgres | postgres:16 | 5432 |
| redis | redis:7-alpine | 6379 |

## Run with Docker

```bash
# Build and start all services
docker-compose up --build

# Run in background
docker-compose up -d

# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

## Run without Docker

```bash
cd DockerDemo.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products | Products + environment info |
| GET | /api/products/health | Health check |

## Project Structure

```
16-docker/
├── docker-compose.yml      ← Orchestrates all services
└── DockerDemo.Api/
    ├── Dockerfile          ← Multi-stage build
    └── Controllers/
        └── ProductsController.cs
```

---