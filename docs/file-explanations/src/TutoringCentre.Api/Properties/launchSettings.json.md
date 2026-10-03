# src/TutoringCentre.Api/Properties/launchSettings.json

## Purpose

Local-development launch profiles for `dotnet run` and IDEs.

## Where It Fits

Read by `dotnet run --launch-profile` and Visual Studio/Rider. Not used in production or tests.

## Walkthrough

Profiles `http` (`applicationUrl http://localhost:5080`) and `https` (`https://localhost:7197;http://localhost:5245`), both `commandName: Project`, `dotnetRunMessages: true`, `launchBrowser: true`, `ASPNETCORE_ENVIRONMENT=Development`. The `http` profile port matches the Vite proxy target and README. `Development` is what turns on startup auto-migration.

## Concepts Used

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#11-configuration--environment--deployment](../../../../PROJECT_OVERVIEW.md#11-configuration--environment--deployment).)

#### Where it appears in this file

`ASPNETCORE_ENVIRONMENT=Development`.

#### How it works here

Both profiles.

#### Why it matters here

Selects `appsettings.Development.json`, user-secrets and auto-migrate.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

`ASPNETCORE_ENVIRONMENT=Development`; URLs/ports 5080, 7197, 5245.

## Gotchas and Issues

`launchBrowser: true` opens the base URL, which has no page (404 problem+json).

## Related Files

- [`src/TutoringCentre.Api/Program.cs`](../Program.cs.md)
- [`frontend/vite.config.ts`](../../../frontend/vite.config.ts.md)
- [`README.md`](../../../README.md.md)
