# Security Policy

## Supported versions

| Version | Supported |
|---|---|
| main branch | yes |

## Reporting a vulnerability

Please report security issues privately through GitHub Security Advisories for this repository.

Do not open public issues for undisclosed vulnerabilities.

## Scope

This repository:

- uses synthetic benchmark data only
- does not collect telemetry
- stores credentials in environment variables or local Docker defaults for development
- must not be configured with production database credentials

## Local defaults

Docker PostgreSQL defaults to development-only credentials documented in `README.md`. Change them before exposing services beyond localhost.
