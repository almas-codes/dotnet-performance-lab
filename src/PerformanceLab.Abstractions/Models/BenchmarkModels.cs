using System;
using System.Collections.Generic;

namespace PerformanceLab.Abstractions.Models;

public sealed record BenchmarkDefinition(
    string Id,
    string Name,
    string Category,
    string Description,
    string Hypothesis,
    IReadOnlyList<BenchmarkVariant> Variants);

public sealed record BenchmarkVariant(
    string Id,
    string Name,
    bool IsBaseline);

public sealed record DatasetProfile(
    string Id,
    long RowCount,
    string Description);

public sealed record EnvironmentProfile(
    string OperatingSystem,
    string Architecture,
    string Processor,
    int LogicalCores,
    int PhysicalCores,
    string Runtime,
    string RuntimeVersion,
    string Jit);

public sealed record MeasurementScope(
    bool IncludesNetwork,
    bool IncludesDatabaseExecution,
    bool IncludesMaterialization,
    bool IncludesSerialization,
    bool IncludesCacheLookup);

public sealed record BenchmarkStatistics(
    double MeanNs,
    double MedianNs,
    double StdDevNs,
    double ErrorNs,
    double MinNs,
    double MaxNs,
    double P95Ns,
    double P99Ns,
    double AllocatedBytes,
    double Gen0,
    double Gen1,
    double Gen2,
    double Ratio,
    double RatioStdDev);

public sealed record BenchmarkResult(
    string BenchmarkId,
    string Variant,
    BenchmarkStatistics Statistics,
    EnvironmentProfile Environment,
    DatasetProfile Dataset,
    MeasurementScope Scope);
