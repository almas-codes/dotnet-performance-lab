# 🚀 .NET Performance Engineering Laboratory (dotnet-performance-lab)

> Reproducible .NET performance benchmarks for real engineering decisions.

Welcome to **dotnet-performance-lab** — a production-grade, meticulously organized performance testing laboratory for .NET 10. 

This repository goes beyond simple micro-benchmarks. It provides a robust, cross-platform CLI tool to execute, compare, and analyze realistic performance variants across Data Access (EF Core vs Dapper vs ADO.NET), Collections, Caching, Serialization, Concurrency, and ASP.NET Core.

## 🌟 Why This Project?

Developers constantly face architectural choices: 
* *Should I use EF Core tracking or no-tracking?*
* *Is Dapper actually faster than ADO.NET for my specific query?*
* *When does a Dictionary outperform a List?*

Most online benchmarks are isolated snippets that don't reflect real-world scenarios. **dotnet-performance-lab** solves this by:
1. **Realistic Datasets:** Using generated, substantial datasets for O(N) operations.
2. **Environment Standardization:** Capturing OS, Architecture, and Runtime parameters accurately.
3. **Reproducibility:** Providing a CLI tool that anyone can run to generate exact, comparable results on their own hardware.
4. **Transparent Methodology:** Separating the measurement from the interpretation.

## 📦 Architecture & Extensibility

This lab is heavily modularized to maintain strict dependency hygiene.

* **`PerformanceLab.Core`** & **`PerformanceLab.Abstractions`**: Defines the rigorous standard for all benchmarks (`IBenchmarkScenario`, `BenchmarkResult`).
* **`PerformanceLab.Data.*`**: Contains the data entities and `DbContext` structures to allow isolated ORM testing.
* **`PerformanceLab.Benchmarks`**: The actual BenchmarkDotNet implementations (Collections, Serialization, EF Core, etc).
* **`PerformanceLab.Cli`**: The frontend console application to invoke specific benchmark categories and generate reports.

## 🛠️ How to Use

1. **Clone & Build:**
   ```bash
   git clone git@github-personal:almas-codes/dotnet-performance-lab.git
   cd dotnet-performance-lab
   dotnet build
   ```

2. **List Available Benchmarks:**
   ```bash
   dotnet run --project src/PerformanceLab.Cli -- list
   ```

3. **Run a Benchmark Category:**
   ```bash
   dotnet run --project src/PerformanceLab.Cli -- run --category DataAccess
   ```
   *Note: For DataAccess benchmarks, a local PostgreSQL instance is required (`localhost:5432`, user: `postgres`, pass: `admin`). The CLI will automatically provision the schema and seed data.*

## 🧩 How to Add New Features or Benchmarks

Adding a new benchmark is simple and highly decoupled:

1. **Create the Benchmark Implementation:**
   In `src/PerformanceLab.Benchmarks/YourCategory/`, create a standard BenchmarkDotNet class.

2. **Implement `IBenchmarkScenario`:**
   In the same file, implement the scenario interface to register the benchmark with the CLI.
   ```csharp
   public class MyNewScenario : IBenchmarkScenario
   {
       public string Id => "my-new-scenario";
       public string Name => "My New Test";
       public string Category => "YourCategory";
       // ... provide Hypothesis and Type
       public Type BenchmarkType => typeof(MyBenchmarkClass);
   }
   ```

3. **Run and Analyze:**
   The CLI automatically discovers types implementing `IBenchmarkScenario`. Simply run the CLI targeting your new category to generate the latest results!

---

*Built with precision for .NET 10. Open-source and ready for production.*
