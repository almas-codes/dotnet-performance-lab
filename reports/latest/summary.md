# Latest Benchmark Results

Generated: 2026-09-26T20:46:42.9593325Z

| Benchmark | Method | Parameters | Mean (ns) | Allocated (B/op) | StdDev (ns) |
|---|---|---|---:|---:|---:|
| `ChannelVsConcurrentQueueBenchmark` | 'Channel (unbounded)' |  | 312,610.86 | 133,936 | 2,038.18 |
| `ChannelVsConcurrentQueueBenchmark` | ConcurrentQueue |  | 79,041.80 | 133,144 | 3,485.04 |
| `ListVsDictionaryLookupBenchmark` | Dictionary.TryGetValue | ItemCount=10 | 2.38 | 0 | 0.11 |
| `ListVsDictionaryLookupBenchmark` | Dictionary.TryGetValue | ItemCount=100 | 2.70 | 0 | 0.01 |
| `ListVsDictionaryLookupBenchmark` | Dictionary.TryGetValue | ItemCount=1000 | 2.77 | 0 | 0.09 |
| `ListVsDictionaryLookupBenchmark` | Dictionary.TryGetValue | ItemCount=100000 | 3.44 | 0 | 0.47 |
| `ListVsDictionaryLookupBenchmark` | List.FirstOrDefault | ItemCount=10 | 4.91 | 0 | 0.24 |
| `ListVsDictionaryLookupBenchmark` | List.FirstOrDefault | ItemCount=100 | 18.96 | 0 | 0.22 |
| `ListVsDictionaryLookupBenchmark` | List.FirstOrDefault | ItemCount=1000 | 210.58 | 0 | 0.35 |
| `ListVsDictionaryLookupBenchmark` | List.FirstOrDefault | ItemCount=100000 | 50,505.59 | 0 | 1,301.89 |
| `ReflectionVsSourceGeneratedJsonBenchmark` | 'Deserialize (reflection)' |  | 528.88 | 368 | 6.07 |
| `ReflectionVsSourceGeneratedJsonBenchmark` | 'Deserialize (source generated)' |  | 541.17 | 368 | 8.15 |
| `ReflectionVsSourceGeneratedJsonBenchmark` | 'Serialize (reflection)' |  | 306.09 | 168 | 2.91 |
| `ReflectionVsSourceGeneratedJsonBenchmark` | 'Serialize (source generated)' |  | 225.33 | 168 | 2.50 |
