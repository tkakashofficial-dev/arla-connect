# Arla Connect — B2B Webshop (Demo)

A headless B2B e-commerce demo built to mirror the architecture of **Arla Connect**:
a Vue single-page app talking to a .NET REST API over SQL Server, where business
customers browse products, place orders, and raise claims.

> Portfolio project. Not affiliated with Arla Foods.

## Tech stack

| Layer    | Technology |
|----------|------------|
| Frontend | Vue 3, TypeScript, Vite, Pinia, Vue Router, PrimeVue |
| Backend  | .NET 10, ASP.NET Core Web API, Clean Architecture + CQRS |
| Data     | SQL Server 2022, EF Core |
| DevOps   | Docker Compose, GitHub Actions CI/CD, Azure-ready |

## Architecture

```
Vue 3 SPA  ->  .NET 10 API  ->  Application (CQRS)  ->  Infrastructure (EF Core)  ->  SQL Server
```

Clean Architecture, four layers:

- **Connect.Domain** — entities, value objects, domain rules (no dependencies)
- **Connect.Application** — CQRS handlers, DTOs, validation, interfaces
- **Connect.Infrastructure** — EF Core, SQL Server, external integrations
- **Connect.Api** — thin controllers, auth, error handling

## Getting started

```bash
# 1. Start the database
docker compose up -d

# 2. Run the API
cd backend/src/Connect.Api
dotnet run

# 3. Run the frontend
cd frontend
npm install
npm run dev
```

## Status

Work in progress — see project phases in the repo history.
