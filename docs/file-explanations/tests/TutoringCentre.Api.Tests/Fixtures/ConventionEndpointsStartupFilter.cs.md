# `tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs`

## Purpose

Declares `ConventionEndpointsStartupFilter`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 562 bytes. Source hash: `38aa09a98d5c64b6923deba6bc0551c1d620aca2e0d26f2892f4768f3be5fd72`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs#L6): `internal sealed class ConventionEndpointsStartupFilter : IStartupFilter`
- [Line 8](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs#L8): `public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs:26](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs#L26)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Api.Tests.Fixtures;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ConventionEndpointsStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        ArgumentNullException.ThrowIfNull(next);

        return app =>
        {
            next(app);
            app.UseEndpoints(ConventionEndpoints.Map);
        };
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
