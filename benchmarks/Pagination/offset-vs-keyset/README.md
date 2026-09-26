# Offset vs Keyset Pagination

## Question

How does pagination latency change as page depth increases?

## Hypothesis

Offset pagination degrades at deeper pages because skipped rows must be scanned. Keyset pagination stays more stable when indexed sort keys are used.

## Workload

100,000 orders, page size 50, pages 1 / 10 / 100 / 1,000.

## Limitations

Uses one sort key shape and one PostgreSQL schema. Other databases and index designs may behave differently.
