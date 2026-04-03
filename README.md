# Product Planner Monorepo

This is the main monorepo scaffolding configured with **Nx**, **React + Vite**, and **.NET 8 Clean Architecture**.

## Infrastructure & Stack

- **Monorepo Manager:** Nx
- **Frontend:** React (Vite), Redux Toolkit, React Router, Tailwind CSS, Jest
- **Backend:** .NET 8 Web API, MediatR, AutoMapper, FluentValidation, Entity Framework Core (SQLite default local)
- **Containerization:** Multi-stage Dockerfiles and `docker-compose.yml`
- **Quality Control:** Husky, ESLint, Prettier, SonarQube ready, GitHub Actions

## Run Locally

To spin up the entire environment via Docker Compose:

```bash
docker-compose up --build
```

This binds the Webapp on `http://localhost:3000` and the API on `http://localhost:5000`.

## Architecture Note

The `.NET` application follows **Clean Architecture** patterns broken down into:

- `libs/backend/domain`
- `libs/backend/application`
- `libs/backend/infrastructure`
- `apps/api`

## Development Scripts

- **UI:** Run `npx nx serve webapp`
- **Lint:** Run `npx nx run-many -t lint`
- **Test UI:** Run `npx nx test webapp`
