# System.Text.Json Reflection vs Source Generated

## Question

Does source generation reduce serialization cost for a known DTO shape?

## Hypothesis

Source-generated serializers avoid repeated reflection and can reduce allocations for hot paths.

## Workload

Same DTO, same naming policy, serialize and deserialize using reflection and source-generated contexts.

## Limitations

Microbenchmark on one payload shape. Large graphs, polymorphic payloads, and custom converters may change the outcome.
