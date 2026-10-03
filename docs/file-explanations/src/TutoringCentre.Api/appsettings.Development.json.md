# src/TutoringCentre.Api/appsettings.Development.json

## Purpose

Development-only overrides of configuration.

## Where It Fits

Loaded after `appsettings.json` when `ASPNETCORE_ENVIRONMENT=Development`.

## Walkthrough

Only a `Logging.LogLevel` block (Default Information, `Microsoft.AspNetCore` Warning) identical to the base file and not read by Serilog, so it currently changes nothing.

## Concepts Used

This file introduces no concept that needs a tutorial beyond what its walkthrough already explains.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None.

## Gotchas and Issues

Effectively redundant.

## Related Files

- [`src/TutoringCentre.Api/appsettings.json`](appsettings.json.md)
- [`src/TutoringCentre.Api/Properties/launchSettings.json`](Properties/launchSettings.json.md)
