# Deploying to Azure

The app is built to run on Azure with minimal change, because it is stateless and
configuration-driven. This document maps each part of the stack to an Azure service and
lists the configuration it needs.

## Service mapping

| Component | Local (dev) | Azure (production) |
|-----------|-------------|--------------------|
| Database | SQL Server 2022 (Docker) | **Azure SQL Database** (same engine) |
| API | `dotnet run` | **Azure App Service** or **Azure Container Apps** |
| Frontend | Vite dev server | **Azure Static Web Apps** |
| Secrets | `appsettings.json` | **App Service config** / **Key Vault** |
| Logs | Console (Serilog) | **Application Insights** |

Because local SQL Server and Azure SQL share the same engine, the EF Core migrations apply
unchanged — only the connection string differs.

## Configuration the API needs in Azure

Set these as App Service application settings (never commit real secrets):

```
ConnectionStrings__DefaultConnection = <Azure SQL connection string>
Jwt__Key      = <strong 32+ char secret, from Key Vault>
Jwt__Issuer   = arla-connect
Jwt__Audience = arla-connect-client
Cors__AllowedOrigins__0 = https://<your-static-web-app>.azurestaticapps.net
```

> `__` (double underscore) is how ASP.NET Core maps environment variables to nested config keys.

## Frontend build config

The frontend reads the API URL from `VITE_API_BASE_URL` at build time. For Azure Static Web
Apps, set it to the deployed API URL:

```
VITE_API_BASE_URL=https://<your-api>.azurewebsites.net/api
```

## Suggested CI/CD extension

The current `ci.yml` builds and tests. To deploy, add a `deploy` job (gated on `main`) that:

1. Publishes the API (`dotnet publish -c Release`) and deploys with `azure/webapps-deploy`.
2. Builds the frontend and deploys with `Azure/static-web-apps-deploy`.
3. Runs `dotnet ef database update` against Azure SQL (or relies on migrate-on-startup).

## Scaling notes

- The API is **stateless** (JWT auth, no server session) → scale out horizontally behind the
  App Service / Container Apps load balancer.
- Read-heavy endpoints already project to DTOs and page results; the next step at higher scale
  is a **Redis cache** (Azure Cache for Redis) in front of the catalogue queries.
- Move long-running work (emails, exports) to a queue (Azure Service Bus) + background worker.
