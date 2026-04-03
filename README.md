# FleetOps Hardware Inventory Monorepo

## Introduction

FleetOps is a hardware inventory management platform built as a monorepo. The product helps teams manage hardware assets, manufacturers, categories, and products through a React frontend and a .NET API following Clean Architecture and CQRS patterns.

This README is the main documentation index for product and technical content.

## Main Index

- Product Documentation
  - [Product Overview](#product-overview)
  - [Core Product Capabilities](#core-product-capabilities)
  - [User-Facing Flows](#user-facing-flows)
- Technical Documentation
  - [Architecture and Stack](#architecture-and-stack)
  - [Repository Structure](#repository-structure)
  - [Environment and Runtime](#environment-and-runtime)
  - [Local Development](#local-development)
  - [Containerized Execution](#containerized-execution)
  - [Quality and Tooling](#quality-and-tooling)
  - [Detailed Technical Docs](#detailed-technical-docs)

## Product Overview

FleetOps provides centralized lifecycle management for hardware inventory:

- Track inventory assets and physical attributes.
- Manage manufacturers and product categories.
- Maintain product catalog relationships.
- Expose inventory operations through a REST API.

## Core Product Capabilities

- Inventory management: create, update, delete, and query hardware inventory items.
- Manufacturer management: maintain vendor/manufacturer master data.
- Category management: maintain product category taxonomy.
- Product management: maintain products linked to category and manufacturer.

## User-Facing Flows

- View inventory dashboard and reference data from the web app.
- Search and inspect inventory records.
- Create and update entities through API-backed workflows.
- Run the full stack locally with Docker for integrated testing.

## Architecture and Stack

- Monorepo manager: Nx
- Frontend: React + Vite + Redux Toolkit + React Router + Tailwind CSS
- Backend: ASP.NET Core Web API on .NET 10, MediatR, AutoMapper, FluentValidation, EF Core (SQLite)
- Pattern: Clean Architecture + CQRS + Result pattern
- Resilience: Polly-based HTTP resilience configuration for external dependencies

## Repository Structure

```text
.
|- apps/
|  |- api/                 # ASP.NET Core API and application/domain/infrastructure layers
|  |- api-tests/           # NUnit-based test project
|  |- webapp/              # React web application
|- docs/                   # Product and technical documentation
|- libs/                   # Shared libraries (current placeholders for shared code)
|- packages/               # Workspace packages
|- nx.json                 # Nx workspace configuration
`- docker-compose.yml      # Local multi-service execution
```

## Environment and Runtime

- API runtime: .NET 10
- Default API database: SQLite (`apps/api/local.db`)
- API connection string source: [apps/api/appsettings.json](apps/api/appsettings.json)
- Web app runtime: Node.js (for local frontend development)

## Local Development

Prerequisites:

- .NET SDK 10
- Node.js LTS + npm

Suggested commands:

```bash
# Restore Node workspace dependencies
npm install

# Build API directly
dotnet build apps/api/Api.csproj -nologo

# Run API
dotnet run --project apps/api/Api.csproj
```

If Nx targets are configured in your local workspace, you can also use Nx task execution:

```bash
npx nx run-many -t lint
npx nx run-many -t test
```

## Containerized Execution

Run full stack with Docker Compose:

```bash
docker-compose up --build
```

Default ports from [docker-compose.yml](docker-compose.yml):

- Web app: `http://localhost:3000`
- API: `http://localhost:5001`

## Quality and Tooling

- Formatting and staged checks: Husky + lint-staged + Prettier
- Commit conventions: Commitlint
- CI baseline: GitHub Actions workflow in [.github/workflows/ci.yml](.github/workflows/ci.yml)
- Static analysis baseline: [sonar-project.properties](sonar-project.properties)

## Detailed Technical Docs

- Main docs index: [docs/index.md](docs/index.md)
- Architecture design: [docs/architecture-design.md](docs/architecture-design.md)
- Database design plan: [docs/db-design-plan.md](docs/db-design-plan.md)
- SQL DDL: [docs/db-ddl.sql](docs/db-ddl.sql)
- ER diagram (PlantUML): [docs/db-er-diagram.puml](docs/db-er-diagram.puml)
- Resilience proposal: [docs/resilience-proposal.md](docs/resilience-proposal.md)
