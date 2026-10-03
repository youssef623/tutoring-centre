# src/TutoringCentre.Application/TutoringCentre.Application.csproj

## Purpose
Project file for the use-case layer. It references **only Domain**, plus four packages: FluentValidation for validators, its DI extensions, and the DI and logging *abstractions*.

## Where it fits
Application layer. Referenced by Infrastructure, Api, Application.Tests and Architecture.Tests. `ProjectReferenceTests.Application_ReferencesOnlyDomain` asserts the reference list. `DependencyRuleTests` forbids EF Core, ASP.NET Core and Npgsql usage.

## Walkthrough
- **Line 3:** `ProjectReference` to Domain.
- **Lines 6–9:**
  - `FluentValidation`
  - `FluentValidation.DependencyInjectionExtensions` (for `AddValidatorsFromAssembly`)
  - `Microsoft.Extensions.DependencyInjection.Abstractions` (`IServiceCollection`, `GetRequiredService`)
  - `Microsoft.Extensions.Logging.Abstractions` (`ILogger<T>`)
- **Line 12:** `InternalsVisibleTo` for Architecture.Tests.

## Concepts used
- **Depending on abstractions packages only:** Application never pulls in a concrete DI container or logging provider.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Application.Tests is **not** granted `InternalsVisibleTo`, so `internal HandlerRegistration` can't be unit-tested directly.
- UTF-8 BOM at the file start (harmless).

## Related files
- [DependencyInjection.cs](DependencyInjection.cs.md)
- [ProjectReferenceTests.cs](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
