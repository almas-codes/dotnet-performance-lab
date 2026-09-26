using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using PerformanceLab.Abstractions.Catalog;

namespace PerformanceLab.Benchmarks.Serialization;

public class ReflectionVsSourceGeneratedJsonScenario : IBenchmarkScenario
{
    public string Id => "reflection-vs-source-generated-json";
    public string Name => "System.Text.Json Reflection vs Source Generated";
    public string Category => "Serialization";
    public string Description => "Serializes and deserializes the same payload using reflection-based and source-generated System.Text.Json.";
    public string Hypothesis => "Source generation reduces reflection and can lower allocations for repeated serialization of known types.";
    public Type BenchmarkType => typeof(ReflectionVsSourceGeneratedJsonBenchmark);
}

public sealed record ProductDto(int Id, string Name, string Category, decimal Price, string Sku, DateTime UpdatedAt);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ProductDto))]
[JsonSerializable(typeof(ProductDto[]))]
internal partial class ProductJsonContext : JsonSerializerContext;

[MemoryDiagnoser]
public class ReflectionVsSourceGeneratedJsonBenchmark
{
    private ProductDto _payload = null!;
    private byte[] _serializedReflection = null!;
    private byte[] _serializedSourceGen = null!;
    private JsonSerializerOptions _reflectionOptions = null!;

    [GlobalSetup]
    public void Setup()
    {
        _payload = new ProductDto(
            Id: 42,
            Name: "Performance Sensor",
            Category: "Instrumentation",
            Price: 129.99m,
            Sku: "PERF-042",
            UpdatedAt: DateTime.UtcNow);

        _reflectionOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _serializedReflection = JsonSerializer.SerializeToUtf8Bytes(_payload, _reflectionOptions);
        _serializedSourceGen = JsonSerializer.SerializeToUtf8Bytes(_payload, ProductJsonContext.Default.ProductDto);
    }

    [Benchmark(Baseline = true, Description = "Serialize (reflection)")]
    public byte[] SerializeReflection()
    {
        return JsonSerializer.SerializeToUtf8Bytes(_payload, _reflectionOptions);
    }

    [Benchmark(Description = "Serialize (source generated)")]
    public byte[] SerializeSourceGenerated()
    {
        return JsonSerializer.SerializeToUtf8Bytes(_payload, ProductJsonContext.Default.ProductDto);
    }

    [Benchmark(Description = "Deserialize (reflection)")]
    public ProductDto DeserializeReflection()
    {
        return JsonSerializer.Deserialize<ProductDto>(_serializedReflection, _reflectionOptions)!;
    }

    [Benchmark(Description = "Deserialize (source generated)")]
    public ProductDto DeserializeSourceGenerated()
    {
        return JsonSerializer.Deserialize(_serializedSourceGen, ProductJsonContext.Default.ProductDto)!;
    }
}
