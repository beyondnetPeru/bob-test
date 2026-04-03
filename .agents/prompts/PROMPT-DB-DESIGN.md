**DIRECTIVE: SCENE 1 - DATABASE ARCHITECTURE & NORMALIZATION**

**Assigned Crew:** @AGENT_01_BACKEND.md
@AGENT_03_ARCHITECT_DEVOPS.md

**Rulebook to Enforce:** @RULES_DATABASE.md
@RULES_BACKEND.md

**Source Material:** Read the raw hardware dataset located in section 2 of @AGENT_CONTEXT.md.

**ACTION REQUIRED:**
Your immediate task is to organize and transform the raw data from the provided table into a highly optimized SQL Server / SQL Express schema. You must apply systematic reasoning to design this for rapid, everyday data access.

**Execution Steps:**

1. **Analyze & Normalize (3NF):** Do NOT create a single flat table. Break the data down into a normalized relational model. Create necessary lookup tables for recurring components (e.g., `CPUs`, `GPUs`, `StorageTypes`, `Manufacturers`).
2. **Standardize Data:** Factor in the rules for data cleansing. Ensure the schema design accounts for standardized units (e.g., weights stored as decimals for `kg`, storage as integers for `GB`).
3. **Performance Optimization:** Define primary keys, foreign key relationships, and essential indexes that will guarantee fast read/write access for a high-traffic SPA dashboard.
4. **Audit Readiness:** Include shadow properties or columns for auditing (`CreatedAt`, `UpdatedAt`) without cluttering the core domain logic.

**Deliverable:** Provide the complete, production-ready SQL DDL (Data Definition Language) script to create this schema, followed by a brief, realistic explanation of why this specific relational structure guarantees fast access for the frontend IT dashboard. Do not write the API code yet; focus entirely on the database foundation.
Also include a design ER diagram for the database schema that we can follow or see using PlantUML
