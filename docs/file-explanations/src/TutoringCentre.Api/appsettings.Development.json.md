# src/TutoringCentre.Api/appsettings.Development.json

## Purpose
Development-environment overrides, loaded when `ASPNETCORE_ENVIRONMENT=Development` (set by `launchSettings.json`). It only repeats the default `Logging.LogLevel` values.

## Where it fits
Api configuration, layered over `appsettings.json`.

## Walkthrough
- **Lines 2–7:** `Logging.LogLevel`: Default Information, `Microsoft.AspNetCore` Warning. These are identical to the base file.

## Concepts used
- **Environment-specific configuration files.**

## Data and control flow
Not applicable.

## Configuration and environment
None effective.

## Gotchas and issues
- **No effect.** The values duplicate the base file, and Serilog ignores the `Logging` section anyway. To change dev log levels, override `Serilog:MinimumLevel` here instead.

## Related files
- [appsettings.json](appsettings.json.md)
- [launchSettings.json](Properties/launchSettings.json.md)
