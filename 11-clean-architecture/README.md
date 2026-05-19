# 11 — Clean Architecture

A production-ready Clean Architecture implementation in .NET 8 with strict layer separation and dependency inversion.

## What You'll Learn
- Clean Architecture layer responsibilities
- Dependency Inversion Principle
- Domain-driven entity design with factory methods
- Repository pattern with interface abstraction

## Layer Responsibilities

| Layer | Responsibility | Dependencies |
|-------|---------------|-------------|
| Domain | Entities, business rules, interfaces | None |
| Application | Use cases, orchestration | Domain only |
| Infrastructure | DB, external services | Domain only |
| Api | HTTP, controllers | Application only |

## How It Works