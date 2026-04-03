# 📖 Hardware Inventory System - Documentation Portal

Welcome to the central index for the Hardware Inventory Management System. This portal consolidates all project documentation, specifications, and execution plans into a single, organized view.

---

## 🏛️ Project Foundations

| Context               | Documents                                              | Description                                                                   |
| :-------------------- | :----------------------------------------------------- | :---------------------------------------------------------------------------- |
| **System Overview**   | [Architecture Design](./architecture-design.md)        | High-level strategy for Clean Architecture, Nx Monorepo, and Design Patterns. |
| **Technical Context** | [AGENT_CONTEXT.md](../.agents/skills/AGENT_CONTEXT.md) | The core instructions, raw dataset, and business case for the project.        |

---

## 📅 Execution Scenes & Tasks

| Progress | Context     | Documents                                 | Description                                                       |
| :------: | :---------- | :---------------------------------------- | :---------------------------------------------------------------- |
|    ✅    | **Scene 1** | [Database Design](./db-design-plan.md)    | 3NF Normalization strategy and unit standardization proposal.     |
|    ✅    | **Scene 1** | [SQL DDL Script](./db-ddl.sql)            | Production-ready SQL DDL for database creation.                   |
|    ✅    | **Scene 1** | [ER Diagram (PUML)](./db-er-diagram.puml) | Visual PlantUML model of the database entities and relationships. |

---

## 📁 Repository Structure Overview

```text
/
├── apps/              # Backend (API) and Frontend (Webapp) applications
├── libs/              # Shared logic (Domain, Application, Infrastructure)
├── docs/              # 👈 You are here: Detailed technical documentation
└── .agents/           # System instructions and raw input datasets
```

---

> [!NOTE]
> This index is updated automatically as new scenes are implemented. All links are relative to the `/docs` directory.
