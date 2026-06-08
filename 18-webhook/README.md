# 18 — Webhook Pattern

A .NET 8 implementation of the Webhook pattern with HMAC signature validation and retry mechanism.

## What You'll Learn
- Webhook pattern fundamentals
- HMAC-SHA256 signature for payload verification
- Retry with exponential backoff
- Delivery logging
- Subscription management

## How It Works

Client subscribes: POST /webhook/subscribe { eventType, targetUrl }
Event triggers: POST /webhook/trigger { eventType, payload }
Service finds active subscribers for that eventType
Sends POST to each targetUrl with HMAC signature
If fails → retry up to 3 times with exponential backoff
Logs every delivery attempt


## HMAC Signature
X-Webhook-Signature: sha256=abc123...
// Receiver verifies:
var expected = ComputeHmac(payload, secret);
var isValid = expected == receivedSignature;

## Retry Strategy
Attempt 1 → fail → wait 2s
Attempt 2 → fail → wait 4s
Attempt 3 → fail → mark as failed


## Project Structure
WebhookPattern.Infrastructure/
├── Models/
│   ├── WebhookSubscription.cs
│   └── WebhookDelivery.cs
└── Services/
└── WebhookService.cs    ← Core logic
WebhookPattern.Api/
└── Controllers/
└── WebhookController.cs

## Run
```bash
cd WebhookPattern.Api
dotnet run
```

## Endpoints
| Method | URL | Description |
|--------|-----|-------------|
| POST | /api/webhook/subscribe | Create subscription |
| DELETE | /api/webhook/{id} | Cancel subscription |
| POST | /api/webhook/trigger | Trigger event |
| GET | /api/webhook/subscriptions | List subscriptions |
| GET | /api/webhook/deliveries | Delivery logs |
| POST | /api/webhook/receiver | Test receiver |

## Packages Used
| Package | Purpose |
|---------|---------|
| HttpClient | Sending webhooks |
| System.Security.Cryptography | HMAC signature |