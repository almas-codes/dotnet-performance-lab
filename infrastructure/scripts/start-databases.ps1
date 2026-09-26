$ErrorActionPreference = "Stop"
$composeFile = Join-Path $PSScriptRoot "..\docker\docker-compose.yml"
docker compose -f $composeFile up -d
Write-Host "PostgreSQL starting on localhost:5432 (database: perf_lab, user: postgres, password: perf_lab_dev)"
