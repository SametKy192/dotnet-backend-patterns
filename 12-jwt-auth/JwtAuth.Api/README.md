
# =============================================
# 12-jwt-auth/README.md
# =============================================

# 12 — JWT Auth + Refresh Token

A .NET 8 implementation of JWT authentication with refresh token rotation, BCrypt password hashing, and role-based authorization.

## What You'll Learn
- JWT access token generation and validation
- Refresh token rotation
- BCrypt password hashing
- Role-based authorization with [Authorize]
- Swagger JWT integration

## How It Works

### Login Flow
```
POST /api/auth/login
    → Verify username/password (BCrypt)
    → Generate JWT access token (15 min)
    → Generate refresh token (7 days) → save to DB
    → Return both tokens
```

### Token Refresh Flow
```
POST /api/auth/refresh
    → Validate refresh token (not used, not revoked, not expired)
    → Mark old token as used (rotation)
    → Generate new access token + refresh token
    → Return new tokens
```

### Protected Endpoint Flow
```
GET /api/auth/me
    → [Authorize] checks JWT
    → Valid → return user info
    → Invalid/expired → 401
```

## Security Features
- Access token: short-lived (15 min)
- Refresh token: long-lived (7 days), single-use
- Token rotation: old refresh token invalidated on use
- BCrypt: password never stored in plain text

## Project Structure

```
JwtAuth.Application/
├── Interfaces/
│   └── ITokenService.cs
└── Models/
    ├── User.cs
    └── RefreshToken.cs

JwtAuth.Infrastructure/
├── Persistence/
│   └── AppDbContext.cs
└── Services/
    ├── TokenService.cs
    └── AuthService.cs

JwtAuth.Api/
└── Controllers/
    └── AuthController.cs
```

## Run

```bash
cd JwtAuth.Api
dotnet run
```

## Endpoints

| Method | URL | Auth | Description |
|--------|-----|------|-------------|
| POST | /api/auth/register | No | Register |
| POST | /api/auth/login | No | Login |
| POST | /api/auth/refresh | No | Refresh token |
| POST | /api/auth/logout | No | Logout |
| GET | /api/auth/me | Yes | Current user |

## Packages Used

| Package | Purpose |
|---------|---------|
| JwtBearer | JWT middleware |
| BCrypt.Net-Next | Password hashing |

---
