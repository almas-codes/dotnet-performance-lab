# EF Core vs Dapper vs ADO.NET

## Question

Under an equivalent read query, how do EF Core, Dapper, and ADO.NET compare?

## Hypothesis

ADO.NET should allocate least. Dapper should stay close. EF Core NoTracking should trail Dapper. EF Core tracking should add overhead.

## Workload

PostgreSQL `Customers` table, parameterized `LIMIT` reads at 100 / 1,000 / 10,000 rows.

## Prerequisites

```bash
./infrastructure/scripts/start-databases.ps1
```

Or set `PERFLAB_POSTGRES` to your connection string.

## Limitations

Results are provider-specific and depend on network, PostgreSQL version, and schema state.
