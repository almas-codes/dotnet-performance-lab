using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using PerformanceLab.Abstractions.Catalog;

namespace PerformanceLab.Benchmarks.Collections;

public class ListVsDictionaryLookupScenario : IBenchmarkScenario
{
    public string Id => "dictionary-vs-list";
    public string Name => "List vs Dictionary Lookup";
    public string Category => "Collections";
    public string Description => "Compares the lookup speed of List<T>.FirstOrDefault vs Dictionary<TKey, TValue> lookup under varying dataset sizes.";
    public string Hypothesis => "For small dataset sizes, the overhead of hashing makes List<T> iteration faster. At a specific inflection point (typically around 10-20 items), Dictionary<TKey, TValue> achieves O(1) superiority.";
    public Type BenchmarkType => typeof(ListVsDictionaryLookupBenchmark);
}

[MemoryDiagnoser]
public class ListVsDictionaryLookupBenchmark
{
    private List<Item> _list = null!;
    private Dictionary<int, Item> _dictionary = null!;
    private int _targetId;

    [Params(10, 100, 1_000, 100_000)]
    public int ItemCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // 1. Deterministic data setup
        var random = new Random(42); 
        _list = Enumerable.Range(0, ItemCount)
            .Select(i => new Item(i, $"Item {i}"))
            .OrderBy(x => random.Next()) // Shuffle so we don't just hit the first element optimally
            .ToList();

        _dictionary = _list.ToDictionary(x => x.Id);

        // Pick a target ID that is guaranteed to be in the dataset,
        // and ideally somewhere in the middle for a realistic average-case list scan.
        _targetId = _list[ItemCount / 2].Id;
    }

    [Benchmark(Baseline = true, Description = "List.FirstOrDefault")]
    public Item? ListLookup()
    {
        return _list.FirstOrDefault(x => x.Id == _targetId);
    }

    [Benchmark(Description = "Dictionary.TryGetValue")]
    public Item? DictionaryLookup()
    {
        _dictionary.TryGetValue(_targetId, out var item);
        return item;
    }

    // A realistic domain entity
    public sealed record Item(int Id, string Name);
}
