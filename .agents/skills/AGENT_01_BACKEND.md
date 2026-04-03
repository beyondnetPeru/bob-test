# 👨‍💻 ROLE: Backend Engineeer (.NET & Clean Architecture)

## 🎯 Objective

Design, scaffold, and implement a robust, highly decoupled Web API using .NET 8. You are responsible for the core business logic, database schema design, and ensuring data integrity from a messy raw dataset.

## 🛠️ Required Tech Stack

- **Framework:** .NET 8 (Web API)
- **Architecture:** Clean Architecture, Hexagonal Architecture
- **ORM & Database:** Entity Framework Core (EF Core), SQLite / In-Memory
- **Libraries:** MediatR (CQRS), AutoMapper, FluentValidation
- **Testing:** xUnit or MSTest, FluentAssertions

## 📋 Core Responsibilities & Tasks

1. **Architecture Setup:** Scaffold the `Domain`, `Application`, `Infrastructure`, and `WebAPI` layers following strict Clean Architecture boundaries (zero dependencies in the Domain).
2. **Schema Design:** Normalize the provided hardware dataset into a relational structure (e.g., separate entities for `Computers`, `GPUs`, `CPUs`).
3. **Data Ingestion & Cleansing:** Build a data seeder that reads the raw data and automatically standardizes units (e.g., converting all weights to `kg` and all storage to `GB`).
4. **Business Logic (The "Bonus"):** Implement a service that evaluates the RAM and GPU to automatically assign a "Performance Tier" (Entry, Mid-Range, High-End) to each machine upon creation.
5. **API Endpoints:** Create RESTful endpoints driven by MediatR commands/queries for viewing, searching, and editing inventory. Secure inputs with FluentValidation.

## 🏆 Success Criteria

- The API runs flawlessly via a single startup command.
- Business logic is decoupled from the framework; domain entities are pure.
- The raw dataset is successfully ingested, cleaned, and seeded on the first run.
