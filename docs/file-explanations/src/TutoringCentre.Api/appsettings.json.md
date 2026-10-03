# src/TutoringCentre.Api/appsettings.json

## Purpose
Base configuration for the API in all environments. It holds logging levels, Serilog levels, allowed hosts, and the `ConnectionStrings:Postgres` key with an **empty** value. The empty value documents the key without committing a secret.

## Where it fits
Api configuration. ASP.NET Core loads it automatically. `Program.cs:11` (`ReadFrom.Configuration`) reads the `Serilog` section, and `Infrastructure/DependencyInjection.cs:30` reads the connection string.

## Walkthrough
- **Lines 2–7, `Logging.LogLevel`:** Default Information, `Microsoft.AspNetCore` Warning. This is template default and **not used** by Serilog.
- **Lines 8–15, `Serilog.MinimumLevel`:** Default Information, override `Microsoft.AspNetCore` Warning. This is the effective log configuration.
- **Line 16:** `AllowedHosts: "*"`. The host-filtering middleware accepts any Host header.
- **Lines 17–19:** `ConnectionStrings.Postgres: ""`. A real value comes from user-secrets or an environment variable. Empty means the readiness check is the always-Unhealthy stub.

## Concepts used
- **Layered configuration:** later sources (user-secrets, env vars) override this file.

## Data and control flow
Not applicable.

## Configuration and environment
See above. The environment-variable form of the key is `ConnectionStrings__Postgres`, standard ASP.NET Core mapping.

## Gotchas and issues
- Log levels are configured in two places, one of them inert (`Logging`).
- `AllowedHosts: "*"` should be narrowed in production.

## Related files
- [appsettings.Development.json](appsettings.Development.json.md)
- [Program.cs](Program.cs.md)
- [Infrastructure DependencyInjection.cs](../TutoringCentre.Infrastructure/DependencyInjection.cs.md)
