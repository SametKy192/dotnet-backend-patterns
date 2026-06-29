# 25 — Feature Flags

A .NET 8 implementation of feature flags using Microsoft.FeatureManagement for runtime feature toggling without redeployment.

## What You'll Learn
- Feature flag fundamentals
- Microsoft.FeatureManagement library
- Code-based feature checks (IsEnabledAsync)
- Attribute-based feature gates ([FeatureGate])
- Environment-specific feature configuration

## Why Feature Flags?
Without Feature Flags:

New feature ready → deploy to production → all users see it → bug found → rollback ❌
With Feature Flags:

New feature ready → deploy → flag=false → no user sees it

→ Test internally → flag=true for 10% → gradual rollout ✅

## Use Cases

| Use Case | Example |
|----------|---------|
| A/B Testing | Show new checkout to 50% of users |
| Canary Release | Enable for internal users first |
| Kill Switch | Disable broken feature without deploy |
| Beta Access | Enable for specific user groups |

## Two Ways to Check

### 1. Code-based (flexible)
```csharp
if (await _featureManager.IsEnabledAsync("NewCheckout"))
{
    // New behavior
}
else
{
    // Old behavior
}
```

### 2. Attribute-based (simple)
```csharp
[HttpGet("beta")]
[FeatureGate("BetaFeature")]  // Returns 404 if disabled
public IActionResult Beta() { }
```

## Configuration

```json
// appsettings.json — production
"FeatureManagement": {
  "NewCheckout": true,
  "AiRecommendations": false,  ← disabled in production
  "BetaFeature": false
}

// appsettings.Development.json — dev
"FeatureManagement": {
  "AiRecommendations": true,   ← enabled in development
  "BetaFeature": true
}
```

## Project Structure
FeatureFlags.Api/

├── Controllers/

│   └── ProductsController.cs   ← Feature flag usage

├── Features/

│   └── FeatureNames.cs         ← Constants (no magic strings)

├── appsettings.json            ← Production flags

└── appsettings.Development.json ← Dev flags

## Run

```bash
cd FeatureFlags.Api
dotnet run
```

Toggle flags in `appsettings.json` and restart — no code change needed.

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products/features | View all flag states |
| GET | /api/products | Products (AI recs if enabled) |
| GET | /api/products/checkout | v1 or v2 based on flag |
| GET | /api/products/beta | 404 if BetaFeature=false |

## Packages Used

| Package | Purpose |
|---------|---------|
| Microsoft.FeatureManagement.AspNetCore | Feature flag management |