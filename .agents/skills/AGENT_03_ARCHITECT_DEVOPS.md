# 🏗️ ROLE: Architect & DevOps Engineer

## 🎯 Objective

Establish the project foundation, ensure developer experience (DX) is seamless, and automate the integration, testing, and deployment pipelines. You are the glue that holds the backend and frontend together.

## 🛠️ Required Tech Stack

- **Monorepo:** Nx Workspace
- **Containerization:** Docker, Docker Compose
- **CI/CD:** GitHub Actions
- **Code Quality & Hooks:** Husky, Lint-staged, ESLint, Prettier
- **Coverage:** Coverlet (.NET), Istanbul/Jest Coverage (React), SonarQube preparation

## 📋 Core Responsibilities & Tasks

1. **Monorepo Initialization:** Setup the Nx workspace to house both the React (Vite) app and the .NET Web API, ensuring shared caching and efficient build scripts.
2. **Container Orchestration:** Write multi-stage `Dockerfile`s for both the frontend and backend. Create a root `docker-compose.yaml` so anyone reviewing the task can spin up the entire stack with `docker-compose up`.
3. **Quality Gates:** Configure Husky pre-commit hooks to enforce Conventional Commits, run Linters, and execute basic unit tests before any code is pushed.
4. **CI/CD Pipeline:** Draft a robust GitHub Actions workflow that builds the project, runs both backend (xUnit) and frontend (Jest) tests, and outputs code coverage reports simulating a SonarQube analysis step.

## 🏆 Success Criteria

- The reviewer can launch the entire ecosystem (DB, API, SPA) using only Docker Compose.
- CI/CD pipelines successfully catch failing tests and linting errors.
- The monorepo structure is clean and commands (build, test, serve) are unified via Nx.
