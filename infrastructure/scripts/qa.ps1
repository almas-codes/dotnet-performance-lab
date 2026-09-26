$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..\..")

Write-Host "=== dotnet-performance-lab Production QA ===" -ForegroundColor Cyan

Write-Host "`n[1/7] Restore & build (Release)" -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build --no-restore -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`n[2/7] Validate benchmark catalog" -ForegroundColor Yellow
dotnet run --project src/PerformanceLab.Cli -c Release --no-build -- validate
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`n[3/7] Unit & integration tests" -ForegroundColor Yellow
dotnet test -c Release --no-build --verbosity minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`n[4/7] Environment fingerprint" -ForegroundColor Yellow
dotnet run --project src/PerformanceLab.Cli -c Release --no-build -- environment | Out-Null
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`n[5/7] Quick in-memory benchmarks" -ForegroundColor Yellow
$memoryBenchmarks = @(
    "dictionary-vs-list",
    "reflection-vs-source-generated-json",
    "channel-vs-concurrent-queue"
)

foreach ($benchmark in $memoryBenchmarks) {
    Write-Host "  Running $benchmark..." -ForegroundColor Gray
    dotnet run --project src/PerformanceLab.Cli -c Release --no-build -- run --benchmark $benchmark --quick
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

Write-Host "`n[6/7] Database benchmarks" -ForegroundColor Yellow
$dbBenchmarks = @(
    "efcore-vs-dapper-vs-adonet",
    "tracking-vs-no-tracking",
    "offset-vs-keyset"
)

$postgresAvailable = Test-NetConnection -ComputerName localhost -Port 5432 -WarningAction SilentlyContinue | Select-Object -ExpandProperty TcpTestSucceeded
if (-not $postgresAvailable) {
    Write-Host "  PostgreSQL not reachable on localhost:5432." -ForegroundColor DarkYellow
    Write-Host "  Start it with: ./infrastructure/scripts/start-databases.ps1" -ForegroundColor DarkYellow
    Write-Host "  Skipping database benchmarks in this QA run." -ForegroundColor DarkYellow
}
else {
    foreach ($benchmark in $dbBenchmarks) {
        Write-Host "  Running $benchmark..." -ForegroundColor Gray
        dotnet run --project src/PerformanceLab.Cli -c Release --no-build -- run --benchmark $benchmark --quick
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
}

Write-Host "`n[7/7] Verify generated reports" -ForegroundColor Yellow
$summaryPath = Join-Path (Get-Location) "reports\latest\summary.md"
$environmentPath = Join-Path (Get-Location) "reports\latest\environment.json"
if (-not (Test-Path $summaryPath)) { throw "Missing reports/latest/summary.md" }
if (-not (Test-Path $environmentPath)) { throw "Missing reports/latest/environment.json" }
Write-Host "  reports/latest/summary.md" -ForegroundColor Green
Write-Host "  reports/latest/environment.json" -ForegroundColor Green

Write-Host "`nProduction QA complete. All checks passed." -ForegroundColor Green
