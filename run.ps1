#!/usr/bin/env pwsh
# Arla Connect - one-command local startup (Windows / PowerShell).
# Starts SQL Server (Docker), the .NET API, and the Vue frontend.
#
#   Usage:  ./run.ps1
#   Stop:   close the two app windows, then run:  docker compose down

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

Write-Host "=== Arla Connect - starting locally ===" -ForegroundColor Green

# 1. Docker must be running
docker info *> $null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker is not running. Start Docker Desktop, then re-run ./run.ps1" -ForegroundColor Red
    exit 1
}

# 2. Database
Write-Host "Starting SQL Server (Docker)..." -ForegroundColor Cyan
docker compose -f "$root\docker-compose.yml" up -d

Write-Host "Waiting for SQL Server to be ready..." -ForegroundColor Cyan
$ready = $false
for ($i = 0; $i -lt 40; $i++) {
    $status = docker inspect --format '{{.State.Health.Status}}' arla-connect-sql 2>$null
    if ($status -eq "healthy") { $ready = $true; break }
    Start-Sleep -Seconds 3
}
if (-not $ready) {
    Write-Host "SQL Server did not become ready in time." -ForegroundColor Red
    exit 1
}
Write-Host "SQL Server is ready." -ForegroundColor Green

# 3. Frontend: first-run setup (env file + dependencies)
$fe = Join-Path $root "frontend"
if (-not (Test-Path "$fe\.env")) {
    Copy-Item "$fe\.env.example" "$fe\.env"
    Write-Host "Created frontend/.env from .env.example" -ForegroundColor Yellow
}
if (-not (Test-Path "$fe\node_modules")) {
    Write-Host "Installing frontend dependencies (first run, may take a minute)..." -ForegroundColor Cyan
    Push-Location $fe; npm install; Pop-Location
}

# 4. Start the API in its own window (applies migrations + seeds on startup)
$api = Join-Path $root "backend\src\Connect.Api"
Write-Host "Starting the API..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$api'; dotnet run --launch-profile http"

# 5. Start the frontend in its own window
Write-Host "Starting the frontend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$fe'; npm run dev"

Write-Host ""
Write-Host "=== Arla Connect is starting up ===" -ForegroundColor Green
Write-Host "  App:      http://localhost:5173"
Write-Host "  API:      http://localhost:5136"
Write-Host "  API docs: http://localhost:5136/scalar/v1"
Write-Host ""
Write-Host "Logins:  buyer  demo@arla-connect.test / Password123!"
Write-Host "         admin  admin@arla.com / Admin123!"
Write-Host ""
Write-Host "To stop: close the two app windows, then run:  docker compose down"
