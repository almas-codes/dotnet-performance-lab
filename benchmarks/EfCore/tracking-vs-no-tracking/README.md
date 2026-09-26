# EF Core Tracking vs NoTracking

## Question

What is the cost of change tracking for repeated read-only queries?

## Hypothesis

`AsNoTracking()` reduces application-side overhead when entities are not updated in the same context.

## Workload

Repeated customer reads at 100 / 1,000 / 5,000 rows with change tracker reset between iterations.

## Limitations

Does not measure update scenarios or identity resolution behavior.
