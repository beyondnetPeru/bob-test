# 🤖 SYSTEM CONTEXT & AGENT DIRECTIVES

**Project:** Hardware Inventory Management System (Developer Interview Task)
**Goal:** Scaffold and build a production-ready, full-stack SPA demonstrating architectural excellence, Clean Code, and "out-of-the-box" thinking.

## 1. Context & Business Case

This project is a technical interview evaluation. The objective is to parse a raw spreadsheet of computer hardware specifications, design a normalized relational database schema for it, and build a Single Page Application (SPA) to view, search, and edit this data.

The final "production" must feel realistic and grounded in everyday use cases—design the UI/UX as if an actual IT support team uses this daily to manage their fleet, ensuring high visual and functional standards.

**Research & Support:** You are encouraged to use search engines and other materials as needed to ensure the highest standards. For any specific questions regarding the requirements, contact x@x.com.

### Domain Clarification Update (Current Direction)

The spreadsheet rows should now be interpreted as **complete computer products/configurations**, not just isolated parts.

- A **Product** in business terms is a sellable/composable machine such as `Desktop PC`, `Laptop`, `Workstation`, or `Mini PC`.
- Each row contains the machine's **typed component selections**: `CPU`, `Power Source`, `Video`, `Ports`, `Hard Disc`, and `RAM`, plus the overall `Weight`.
- The UI must support:
  1. listing products by **device category**,
  2. showing the specs in the same business-friendly layout as the original spreadsheet,
  3. composing a computer by selecting one or more components by type,
  4. separating **catalog templates** from **real physical inventory assets**.

> Important for seed compatibility: keep the raw markdown table below with the original column labels (`Memory`, `Storage`, `GPU`, `PSU`) even though the business-facing UI should present them as `RAM`, `Hard Disc`, `Video`, and `Power Source`.

## 2. Original Requirements List

Using the data in the Spreadsheet provided:

- **Database:** Organise the data into a SQL Express (or any relational) database.
  - Create a schema that you feel makes sense of the data and likely use cases thereof.
- **SPA:** Create a Single Page Application (SPA) web site/page.
  - Use whichever frameworks/Design patterns you want (e.g., React, Web APIs, etc.).
  - Enables **Viewing/Editing** the data.
  - Enables **Searching** for data.

## 3. Raw Input Data (For Seeding & Schema Design)

The following is the exact raw data provided. **Agent Note:** Notice the inconsistencies (e.g., `kg` vs `lb`, `MB` vs `GB`, `SSD` vs `SDD`). Your backend parsing/seeding logic MUST clean, normalize, and standardize this data.

| Memory | Storage    | Ports                                  | GPU                     | Weight  | PSU        | CPU                                           |
| :----- | :--------- | :------------------------------------- | :---------------------- | :------ | :--------- | :-------------------------------------------- |
| 8 GB   | 1 TB SSD   | 2 x USB 3.0, 4 x USB 2.0               | NVIDIA GeForce GTX 770  | 8.1 kg  | 500 W PSU  | Intel® Celeron™ N3050 Processor               |
| 16 GB  | 2 TB HDD   | 3 x USB 3.0, 4 x USB 2.0               | NVIDIA GeForce GTX 960  | 12 kg   | 500 W PSU  | AMD FX 4300 Processor                         |
| 8 GB   | 3 TB HDD   | 4 x USB 3.0, 4 x USB 2.0               | Radeon R7360            | 16 lb   | 450 W PSU  | AMD Athlon Quad-Core APU Athlon 5150          |
| 16 GB  | 4 TB HDD   | 5 x USB 2.0, 4 x USB 3.0               | NVIDIA GeForce GTX 1080 | 13.8 lb | 500 W PSU  | AMD FX 8-Core Black Edition FX-8350           |
| 32 GB  | 750 GB SDD | 2 x USB 3.0, 2 x USB 2.0, 1 x USB C    | Radeon RX 480           | 7 kg    | 1000 W PSU | AMD FX 8-Core Black Edition FX-8370           |
| 32 GB  | 2 TB SDD   | 2 x USB C, 4 x USB 3.0                 | Radeon R9 380           | 6 kg    | 450 W PSU  | Intel Core i7-6700K 4GHz Processor            |
| 8 GB   | 2 TB HDD   | 8 x USB 3.0                            | NVIDIA GeForce GTX 1080 | 15 lb   | 1000 W PSU | Intel® Core™ i5-6400 Processor                |
| 16 GB  | 500 GB SDD | 4 x USB 2.0                            | NVIDIA GeForce GTX 770  | 8 lb    | 750 W PSU  | Intel® Core™ i5-6400 Processor                |
| 2 GB   | 2 TB HDD   | 10 x USB 3.0, 10 x USB 2.0, 10 x USB C | AMD FirePro W4100       | 9 kg    | 508 W PSU  | Intel Core i7 Extreme Edition 3 GHz Processor |
| 512 MB | 80 GB SSD  | 19 x USB 3.0, 4 x USB 2.0              | Radeon RX 480           | 22 lb   | 700 W PSU  | Intel® Core™ i5-6400 Processor                |

## 3. Strict Technology Stack

Agents must strictly adhere to the following tools. Do not substitute unless explicitly required for compatibility.

- **Workspace:** Nx Monorepo (latest stable).
- **Infrastructure:** Docker & `docker-compose.yaml` (for easy 1-click local setup).
- **Backend:** .NET C# Web API (latest stable).
  - **ORM:** Entity Framework Core (In-Memory or SQLite for this local demo).
  - **Packages:** MediatR (CQRS), AutoMapper, FluentValidation.
- **Frontend:** React + Vite (latest stable).
  - **State & Data:** Redux Toolkit (RTK).
  - **Styling & UI:** Tailwind CSS, React Router.
- **DevOps & QA:**
  - **Frontend Testing:** Jest + React Testing Library.
  - **Backend Testing:** xUnit/MSTest + FluentAssertions + Coverlet (Coverage).
  - **Pre-commit:** Husky (Linting, formatting, basic tests).
  - **CI/CD:** GitHub Actions workflow (Build, Test, SonarQube/Coverage simulation).

## 4. Architectural Constraints

- **Backend Architecture:** Implement **Clean Architecture** combined with **Hexagonal Architecture**. Enforce strict boundaries:
  1.  `Domain` (Entities, Interfaces - Zero dependencies).
  2.  `Application` (Use Cases, CQRS Handlers via MediatR, DTOs).
  3.  `Infrastructure` (EF Core DbContext, Repositories).
  4.  `WebAPI` (Controllers/Minimal APIs mapping to MediatR).
- **Database Schema:** Do not use a single flat table. Normalize entities appropriately to reflect both the **finished device** and its **component composition**.
  - Recommended target model:
    1. `DeviceCategory` (`Desktop PC`, `Laptop`, `Workstation`, etc.)
    2. `CatalogProduct` / `DeviceModel` (the composed computer the user browses)
    3. `ComponentType` (`CPU`, `RAM`, `Storage`, `GPU`, `PSU`, `Port`)
    4. `ComponentCatalogItem` (individual selectable parts)
    5. `ProductComponentSelection` (join table connecting a computer to its chosen parts and quantities)
    6. `InventoryAsset` (real owned/managed device instances)
- **Design Principles:** SOLID, DRY, and clean, self-documenting code. Prefer clear names that distinguish **device products** from **component products** to avoid ambiguity.

## 5. Functional Requirements

1.  **View Data:** Present the hardware inventory in an intuitive, realistic UI (e.g., a datagrid or detailed card view) that an everyday user would find natural.
2.  **Edit Data:** Allow modification of existing records with robust form validation (frontend) and FluentValidation (backend).
3.  **Search/Filter Data:** Implement a dynamic search capability (e.g., search by GPU, filter by RAM size, filter products by category such as `Desktop PC` or `Laptop`).
4.  **Compose a Device:** Allow the user to build or edit a computer by selecting components by type (`CPU`, `RAM`, `Storage`, `GPU`, `PSU`, `Ports`).
5.  **Seed Data:** The application must automatically seed the database using the raw data provided above upon startup.

## 6. "Think Outside the Box" (Bonus Objectives for the Agent)

To stand out, the agent must implement these automated value-adds:

- **Data Cleansing Pipeline:** Write logic in the backend to detect `MB` vs `GB` and `kg` vs `lb`, standardizing them into a single unit type before saving to the DB.
- **Performance Tiering:** Automatically calculate and assign a "Tier" (Entry, Mid-Range, High-End, Workstation) based on the RAM and GPU combination.

## 7. Execution Order for Agents

1.  **Phase 1:** Initialize Nx Monorepo and Docker orchestration.
2.  **Phase 2:** Scaffold .NET Clean Architecture layers and EF Core SQLite context.
3.  **Phase 3:** Create Domain entities and the Data Seeder to parse the raw markdown table data.
4.  **Phase 4:** Build MediatR commands/queries and WebAPI endpoints.
5.  **Phase 5:** Scaffold React/Vite frontend, configure Redux/Tailwind, and build the Everyday-Life IT Dashboard.
6.  **Phase 6:** Setup Husky, Jest, xUnit, and the GitHub Actions CI/CD pipeline.
