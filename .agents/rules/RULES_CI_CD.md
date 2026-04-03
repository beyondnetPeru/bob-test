---
trigger: always_on
---

# 🎬 PRODUCTION RULE: CI/CD & DEVOPS (Nx, Docker, Actions)

## @agent-directive: Nx Monorepo Optimization

- **CONSTRAINT:** Do not run CI tasks on the entire repository unnecessarily. The GitHub Actions pipeline MUST utilize Nx Affected commands (`nx affected:test`, `nx affected:build`) to execute tasks only on changed projects and their dependents.
- **REQUIREMENT:** Enable Nx remote caching in the CI pipeline to drastically reduce build times.

## @agent-directive: Secure & Optimized Containerization

- **REQUIREMENT:** `Dockerfile`s must be multi-stage. The final production stage must use a lightweight base image (e.g., Alpine or Distroless).
- **CONSTRAINT:** Run applications inside Docker containers as non-root users to enforce security best practices. Use a comprehensive `.dockerignore` file.

## @agent-directive: Zero-Tolerance Quality Pipeline

- **CONSTRAINT:** Husky pre-commit hooks MUST enforce Conventional Commits, run Linters, and execute localized unit tests. Commits that fail formatting are rejected.
- **REQUIREMENT:** CI workflows must include a dependency caching step to speed up executions. Failed code coverage gates (e.g., dropping below 80%) MUST block Pull Requests from being merged.
