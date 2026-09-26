# Contributing

1. Add a BenchmarkDotNet class under `src/PerformanceLab.Benchmarks/<Category>/`.
2. Implement `IBenchmarkScenario` in the same file.
3. Add a README under `benchmarks/<Category>/<benchmark-id>/`.
4. Run `dotnet run --project src/PerformanceLab.Cli -- validate`.

Database benchmarks should use `PostgresBenchmarkFixture` and document Docker setup in the benchmark README.
