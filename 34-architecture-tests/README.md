# 34 — Architecture Tests with NetArchTest

An implementation of **Architecture Testing** in a Clean Architecture structure using **NetArchTest.Rules**. This project demonstrates how to enforce design rules and layer dependencies automatically using unit tests.

## What You'll Learn
- What Architecture Testing is and why it's crucial for long-term project maintainability
- How to use `NetArchTest.Rules` to analyze assembly references
- Enforcing Clean Architecture boundary rules (e.g., Domain must not depend on Application/Infrastructure/API)
- Enforcing class naming conventions (e.g., Controllers must end with "Controller")
- Enforcing inheritance rules (e.g., Controllers must inherit from `ControllerBase`)

---

## What is Architecture Testing?

As projects grow and multiple developers contribute, architectural boundaries tend to erode. A developer might accidentally reference database-specific types inside Domain entities, or inject a repository direct into a controller instead of using MediatR/Services.

**Architecture Tests** solve this by writing unit tests that analyze project assemblies using reflection to assert dependency boundaries, naming conventions, and structural rules. If a developer violates a rule, the build pipeline fails immediately.

---

## Key Rules Checked in this Demo

1. **Domain Layer Independence**:
   - `Domain` assembly should not have dependencies on any other assemblies (`Application`, `Infrastructure`, `Api`).
2. **Application Layer Isolation**:
   - `Application` assembly should not depend on `Infrastructure` or `Api`. (It only depends on `Domain`).
3. **Infrastructure Isolation**:
   - `Infrastructure` assembly should not depend on `Api`.
4. **API Controller Naming & Inheritance**:
   - Any class ending in `Controller` in the `Api` assembly must inherit from `ControllerBase`.
   - Any class inheriting from `ControllerBase` must end in `Controller`.

---

## Implementation Example (Application Isolation)

```csharp
[Fact]
public void Application_Should_Not_HaveDependencyOnInfrastructureOrApi()
{
    var applicationAssembly = typeof(Application.IAssemblyMarker).Assembly;

    var result = Types.InAssembly(applicationAssembly)
        .ShouldNot()
        .HaveDependencyOnAny("ArchitectureTests.Infrastructure", "ArchitectureTests.Api")
        .GetResult();

    Assert.True(result.IsSuccessful, "Application layer must not depend on Infrastructure or Api.");
}
```

---

## Running the Tests

```bash
cd ArchitectureTests.Tests
dotnet test
```

Observe that all architecture checks pass. Try introducing an invalid reference (e.g., referencing `ArchitectureTests.Infrastructure` in `ArchitectureTests.Application`) and run `dotnet test` again to see it fail!
