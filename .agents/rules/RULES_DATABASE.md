---
trigger: always_on
---

# 🎬 PRODUCTION RULE: DATABASE (EF Core, Relational Design)

## @agent-directive: EF Core Performance Optimization

- **CONSTRAINT:** All database I/O MUST be asynchronous (`ToListAsync`, `FirstOrDefaultAsync`).
- **CONSTRAINT:** All read-only queries (CQRS Queries) MUST use `.AsNoTracking()` to bypass the EF Core change tracker and maximize performance.
- **REQUIREMENT:** Prevent N+1 queries. Use explicitly defined `.Include()` and `.ThenInclude()` for eager loading related hardware specs, or use compiled queries for hot paths.

## @agent-directive: Schema Design & Integrity

- **REQUIREMENT:** Design schemas to 3rd Normal Form (3NF). Extract CPUs, GPUs, and Manufacturers into lookup tables.
- **CONSTRAINT:** Do not use Data Annotations on Domain Entities. Use the EF Core Fluent API (`IEntityTypeConfiguration`) in the Infrastructure layer to configure keys, relationships, and column types.

## @agent-directive: Auditing & Seeding

- **REQUIREMENT:** Utilize EF Core Shadow Properties to automatically track audit fields (`CreatedAt`, `UpdatedAt`) without polluting the Domain Entities.
- **CONSTRAINT:** Seed data migrations MUST be idempotent (`HasData`). The startup seeder must check for existing records before inserting to prevent duplication.
