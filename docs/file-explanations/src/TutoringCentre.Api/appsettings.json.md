# src/TutoringCentre.Api/appsettings.json

## Purpose

Base application configuration: logging levels, allowed hosts and the (empty) connection-string key.

## Where It Fits

Loaded automatically by `WebApplication.CreateBuilder`. Read by Serilog (`ReadFrom.Configuration`) and `AddInfrastructure` (`GetConnectionString`).

## Walkthrough

`Logging.LogLevel` (Default Information, `Microsoft.AspNetCore` Warning) - legacy Microsoft logging section, effectively unused because Serilog replaces the provider. `Serilog.MinimumLevel` Default Information, Override `Microsoft.AspNetCore` Warning - this is the one that applies. `AllowedHosts: "*"` (any Host header accepted). `ConnectionStrings.Postgres: ""` - documents the key; real value comes from user-secrets or `ConnectionStrings__Postgres` env var (later configuration sources override this file).

## Concepts Used

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#11-configuration--environment--deployment](../../../PROJECT_OVERVIEW.md#11-configuration--environment--deployment).)

#### Where it appears in this file

Empty connection string.

#### How it works here

`ConnectionStrings.Postgres`.

#### Why it matters here

No secret in git; but an unset value stops startup because of `ValidateOnStart`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Keys: `Serilog:MinimumLevel:*`, `AllowedHosts`, `ConnectionStrings:Postgres` (secret when set).

## Gotchas and Issues

`AllowedHosts: *` disables host filtering. Duplicate `Logging` section is redundant.

## Related Files

- [`src/TutoringCentre.Api/appsettings.Development.json`](appsettings.Development.json.md)
- [`src/TutoringCentre.Api/Program.cs`](Program.cs.md)
- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](../TutoringCentre.Infrastructure/DependencyInjection.cs.md)
