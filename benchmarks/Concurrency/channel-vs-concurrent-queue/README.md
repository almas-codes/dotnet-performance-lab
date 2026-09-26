# Channel vs ConcurrentQueue

## Question

How do `Channel<T>` and `ConcurrentQueue<T>` compare for a single-producer / single-consumer workload?

## Hypothesis

Throughput should be comparable for in-memory steady-state transfer, with Channel providing better async integration for real pipelines.

## Workload

Transfer 10,000 integers through each queue type.

## Limitations

Does not model backpressure, multiple producers/consumers, or async I/O.
