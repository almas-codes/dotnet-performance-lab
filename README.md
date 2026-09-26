# dotnet-performance-lab

> Reproducible .NET performance benchmarks for real engineering decisions.

[![CI](https://github.com/almas-codes/dotnet-performance-lab/actions/workflows/ci.yml/badge.svg)](https://github.com/almas-codes/dotnet-performance-lab/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![Benchmarks](https://img.shields.io/badge/benchmarks-6-blue)

This repository does not tell you what to optimize. It shows you how to measure.

Every benchmark includes source code, a stated hypothesis, methodology notes, and reproducible BenchmarkDotNet artifacts.

## Highlights

- **6 production-quality benchmark scenarios** across collections, serialization, concurrency, EF Core, pagination, and data access
- **CLI-first workflow** with catalog validation, environment fingerprinting, and report generation
- **PostgreSQL lab** via Docker Compose for realistic database benchmarks
- **Automated QA** (`infrastructure/scripts/qa.ps1`) and GitHub Actions CI
- **Blazor dashboard** for browsing the benchmark catalog

## Quick start

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) (`global.json` pins the SDK)
- [Docker](https://www.docker.com/) for PostgreSQL database benchmarks

### Build and test

```bash
git clone https://github.com/almas-codes/dotnet-performance-lab.git
cd dotnet-performance-lab
dotnet restore
dotnet build
dotnet test
dotnet run --project src/PerformanceLab.Cli -- validate
```

### Full production QA

```powershell
./infrastructure/scripts/start-databases.ps1
./infrastructure/scripts/qa.ps1
```

## CLI

```bash
dotnet run --project src/PerformanceLab.Cli -- list
dotnet run --project src/PerformanceLab.Cli -- environment
dotnet run --project src/PerformanceLab.Cli -- run --benchmark dictionary-vs-list --quick
dotnet run --project src/PerformanceLab.Cli -- run --category EfCore --quick
dotnet run --project src/PerformanceLab.Cli -- clean
```

Use `--quick` for development/QA. Omit it for standard reproducible runs.

## Benchmark catalog

| ID | Category | Question |
|---|---|---|
| `dictionary-vs-list` | Collections | When does dictionary lookup beat list scan? |
| `reflection-vs-source-generated-json` | Serialization | Does source generation reduce JSON cost? |
| `channel-vs-concurrent-queue` | Concurrency | How do queue primitives compare for SPSC throughput? |
| `efcore-vs-dapper-vs-adonet` | DataAccess | How do ORM and micro-ORM compare for equivalent reads? |
| `tracking-vs-no-tracking` | EfCore | What is tracking overhead for read-only queries? |
| `offset-vs-keyset` | Pagination | How does page depth affect offset vs keyset pagination? |

Methodology: [`docs/methodology/fair-comparison.md`](docs/methodology/fair-comparison.md)

Per-benchmark README files: [`benchmarks/`](benchmarks/)

## Architecture

```text
PerformanceLab.Cli
        ↓
PerformanceLab.Benchmarks   (BenchmarkDotNet scenarios)
        ↓
PerformanceLab.Core         (catalog, config, validation, environment)
        ↓
PerformanceLab.Abstractions
```

Supporting libraries:

- `PerformanceLab.Data.*` — entities and database adapters
- `PerformanceLab.Analysis` — result interpretation helpers
- `PerformanceLab.Reporting` — markdown summary generation
- `PerformanceLab.Web` — benchmark catalog dashboard

## Database lab

```powershell
./infrastructure/scripts/start-databases.ps1
```

Default connection:

```text
Host=localhost;Port=5432;Database=perf_lab;Username=postgres;Password=perf_lab_dev
```

Override with `PERFLAB_POSTGRES`.

## Results

```text
reports/latest/environment.json
reports/latest/summary.md
reports/latest/raw/
```

Machine-specific timings are not checked into git.

## Web dashboard

```bash
dotnet run --project src/PerformanceLab.Web
```

## Contributing

See [`CONTRIBUTING.md`](CONTRIBUTING.md).

## Security

See [`SECURITY.md`](SECURITY.md).

## License

MIT — see [`LICENSE`](LICENSE).
