# `src/TutoringCentre.Application/DependencyInjection.cs`

## Purpose

Source summary (intent, not independent execution proof): Registers the Application layer's services. Called once from the Api's composition root.

## Where it fits

Category: **source**. [Open current file](../../../../src/TutoringCentre.Application/DependencyInjection.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 1,175 bytes. Source hash: `24d4a51f4e612655bf39f88da59410b68f06f21f99418db8b54a8c3035a269e6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../src/TutoringCentre.Application/DependencyInjection.cs#L8): `public static class DependencyInjection`
- [Line 10](../../../../src/TutoringCentre.Application/DependencyInjection.cs#L10): `public static IServiceCollection AddApplication(this IServiceCollection services)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application;

/// <summary>Registers the Application layer's services. Called once from the Api's composition root.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // One actor context per scope (HTTP request or job). ICurrentActor resolves to the SAME instance, read-only.
        services.AddScoped<CurrentActorContext>();
        services.AddScoped<ICurrentActor>(provider => provider.GetRequiredService<CurrentActorContext>());

        // One correlation context per scope. ICorrelationContext resolves to the SAME instance, read-only.
        services.AddScoped<CorrelationContext>();
        services.AddScoped<ICorrelationContext>(provider => provider.GetRequiredService<CorrelationContext>());

        services.AddCqrsHandlers(typeof(AssemblyMarker).Assembly);
        services.AddScoped<Dispatcher>();

        return services;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
