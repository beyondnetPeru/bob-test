# Architectural Design & Solution Definition

This document outlines the architectural patterns, technical stack, and design decisions implemented for the **Product Planner** monorepo.

## 1. Requirement Overview

The solution was required to be a production-ready starter that adheres to modern full-stack standards:

- **Monorepo Management:** Unified workspace for frontend and backend.
- **Backend Architecture:** Multi-layered Clean Architecture with Hexagonal principles.
- **Frontend Architecture:** Modern React SPA with state management and utility-first styling.
- **DevOps:** Fully containerized setup with CI/CD and security monitoring readiness.

---

## 2. Global Architecture (Monorepo)

The project uses **Nx** to manage the monorepo. This provides:

- **Project Isolation:** Distinct boundaries between the Web UI and the API.
- **Shared Tooling:** Unified linting, formatting (Prettier/ESLint), and testing infrastructure.
- **Task Orchestration:** Optimized builds and test execution across the entire stack.

---

## 3. Backend: Clean & Hexagonal Architecture

The .NET 8 backend is structured into four distinct layers to ensure a separation of concerns and maintainability.

### A. Domain Layer (`libs/backend/domain`)

- **Core Entities:** Plain Old CLR Objects (POCOs) representing business concepts (e.g., `Product`).
- **Abstractions:** Repository interfaces (`IProductRepository`) that define how the domain interacts with the outside world without knowing implementation details.
- **Logic:** Business rules and domain services.

### B. Application Layer (`libs/backend/application`)

- **CQRS (MediatR):** Handles commands and queries, isolating side effects from data retrieval.
- **Validation (FluentValidation):** Ensures incoming requests meet the required business schema before processing.
- **Mapping (AutoMapper):** Decouples internal domain entities from external Data Transfer Objects (DTOs).

### C. Infrastructure Layer (`libs/backend/infrastructure`)

- **Persistence:** Implementation of repository interfaces using **Entity Framework Core**.
- **Data Providers:** Support for **PostgreSQL** (production), **SQLite** (local dev), and **Mock/Stub** providers for testing.
- **Dependency Injection:** Repositories are injectable, allowing the application to switch data sources via configuration (`DatabaseProvider` setting).

### D. WebAPI Layer (`apps/api`)

- **Entry Point:** ASP.NET Core Web API controllers.
- **Middleware:** Implements cross-cutting concerns like **Health Checks**, **Rate Limiting**, and **Security Headers**.
- **Cross-Platform:** Multi-stage Dockerfiles ensure the API can run in any container environment.

---

## 4. Frontend: React & Vite

The frontend is a high-performance Single Page Application (SPA).

- **Vite:** Used for rapid development and optimized production bundling.
- **TypeScript 6:** Enforces strict type safety across the entire UI.
- **Redux Toolkit (RTK):** Provides predictable state management.
- **Tailwind CSS v4:** Utilizes utility-first styling for a maintainable and responsive design system.
- **React Router:** Manages navigation and deep-linking.

---

## 5. Security & OWASP Compliance

The solution implements defense-in-depth security:

- **Security Headers:** Strict Content Security Policy (CSP), HSTS, and X-Frame-Options configured in both .NET middleware and **Nginx** (via a hardened `nginx.conf`).
- **Rate Limiting:** Protects the API against brute-force and DoS attacks.
- **Input Sanitization:** React's default protection against XSS, complemented by backend validation.
- **Server Hardening:** Nginx is configured to hide version tokens (`server_tokens off`).

---

## 6. Development Workflow (DevOps)

- **Containerization:** `docker-compose` orchestrates the API and WebApp within a shared bridge network.
- **Quality Gates:** **Husky** hooks enforce linting and conventional commits before code enters the repository.
- **Self-Provisioning:** The system automatically initializes the SQLite schema on first run, providing a "Zero-Config" onboarding experience for developers.
