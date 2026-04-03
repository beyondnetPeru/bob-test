---
trigger: always_on
---

# 🎬 PRODUCTION RULE: BACKEND (.NET 8, Clean Architecture)

## @agent-directive: Clean Architecture & SOLID Principles

- **CONSTRAINT:** The `Domain` layer MUST remain pure with zero external package dependencies. Enforce the Dependency Inversion Principle (DIP) via strict interfaces.
- **REQUIREMENT:** Seal all classes by default (`public sealed class`) unless inheritance is explicitly required. Leverage .NET 8 Primary Constructors to reduce boilerplate.

## @agent-directive: CQRS & MediatR Best Practices

- **REQUIREMENT:** Implement strict CQRS. Separate `Commands` (state mutations) from `Queries` (reads) completely.
- **CONSTRAINT:** Do not duplicate validation in controllers. Implement MediatR `IPipelineBehavior` to intercept requests and run `FluentValidation` automatically. Fail fast and throw standard domain exceptions.
- **REQUIREMENT:** Use a Global Exception Handler Middleware to catch exceptions and return standardized RFC 7807 Problem Details responses.

## @agent-directive: Clean Code & API Standards

- **REQUIREMENT:** Keep Web API Controllers/Minimal APIs extremely thin—they should only route HTTP requests to MediatR and return standard HTTP status codes.
- **CONSTRAINT:** Never expose Domain Entities directly to the API surface. Map entities to Data Transfer Objects (DTOs) using AutoMapper.
