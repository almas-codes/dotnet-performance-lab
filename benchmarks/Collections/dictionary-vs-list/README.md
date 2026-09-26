# List vs Dictionary Lookup

## Question

When does `Dictionary<TKey, TValue>` outperform scanning a `List<T>` for lookup?

## Hypothesis

For small collections, list scan can win due to hashing overhead. At larger sizes, dictionary lookup should dominate.

## Workload

Deterministic shuffled list and dictionary with identical items. Lookup target is near the middle of the collection.

## Variants

- `List.FirstOrDefault`
- `Dictionary.TryGetValue`

## Measurement Scope

In-memory only. Setup excluded from timing.

## Limitations

Microbenchmark only. Real workloads may include different memory locality, struct vs class shapes, and concurrent access patterns.
