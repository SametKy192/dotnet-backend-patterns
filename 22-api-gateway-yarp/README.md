# 22 — API Gateway with YARP

A .NET 8 API Gateway implementation using YARP (Yet Another Reverse Proxy) with routing, load balancing, and centralized logging.

## What You'll Learn
- What an API Gateway is and why it's needed
- YARP configuration with appsettings.json
- Request routing and path transformation
- Load balancing strategies
- Centralized logging at gateway level

## Architecture
Client

→ Gateway (:5000)

→ /gateway/orders/**   → OrderService (:5001)

→ /gateway/products/** → ProductService (:5002)

## Why API Gateway?
Without Gateway:

Client → knows OrderService URL

Client → knows ProductService URL

Client → handles auth for each service
With Gateway:

Client → only knows Gateway URL

Gateway → routes to correct service

Gateway → handles auth centrally

Gateway → logs all requests

## YARP Configuration

```json
"Routes": {
  "orders-route": {
    "ClusterId": "orders-cluster",
    "Match": { "Path": "/gateway/orders/{**catch-all}" },
    "Transforms": [{ "PathRemovePrefix": "/gateway" }]
  }
},
"Clusters": {
  "orders-cluster": {
    "LoadBalancingPolicy": "RoundRobin",
    "Destinations": {
      "order-1": { "Address": "http://localhost:5001" },
      "order-2": { "Address": "http://localhost:5003" }
    }
  }
}
```

## Load Balancing Policies

| Policy | Description |
|--------|-------------|
| RoundRobin | Rotate between destinations |
| LeastRequests | Send to least busy |
| Random | Random selection |
| PowerOfTwoChoices | Best of two random |

## Run

```bash
# Terminal 1 — OrderService
cd YarpGateway.OrderService
dotnet run --urls=http://localhost:5001

# Terminal 2 — ProductService
cd YarpGateway.ProductService
dotnet run --urls=http://localhost:5002

# Terminal 3 — Gateway
cd YarpGateway.Gateway
dotnet run --urls=http://localhost:5000
```

## Project Structure
YarpGateway.Gateway/

├── Middleware/

│   └── GatewayLoggingMiddleware.cs

├── appsettings.json    ← YARP routing config

└── Program.cs
YarpGateway.OrderService/

└── Controllers/

└── OrdersController.cs
YarpGateway.ProductService/

└── Controllers/

└── ProductsController.cs

## Endpoints (via Gateway)

| Method | URL | Routes To |
|--------|-----|-----------|
| GET | /gateway/orders | OrderService /api/orders |
| GET | /gateway/orders/{id} | OrderService /api/orders/{id} |
| GET | /gateway/products | ProductService /api/products |

## Packages Used

| Package | Purpose |
|---------|---------|
| Yarp.ReverseProxy | Reverse proxy + routing |