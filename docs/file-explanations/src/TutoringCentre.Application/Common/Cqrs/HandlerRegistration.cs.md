# `src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs`

## Purpose

Source summary (intent, not independent execution proof): Scans an assembly once and registers handlers (scoped) and validators.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,411 bytes. Source hash: `cf3343f79b0acadae6673ef0bcdab59897724f2fceb095cc936afed711cb4795`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Reflection;`
- Line 2: `using FluentValidation;`
- Line 3: `using Microsoft.Extensions.DependencyInjection;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs#L8): `internal static class HandlerRegistration`
- [Line 10](../../../../../../src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs#L10): `internal static IServiceCollection AddCqrsHandlers(this IServiceCollection services, Assembly assembly)`
- [Line 32](../../../../../../src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs#L32): `private static bool IsHandlerInterface(Type type) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>Scans an assembly once and registers handlers (scoped) and validators.</summary>
internal static class HandlerRegistration
{
    internal static IServiceCollection AddCqrsHandlers(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var implementations = assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false });

        foreach (var implementation in implementations)
        {
            foreach (var handlerInterface in implementation.GetInterfaces().Where(IsHandlerInterface))
            {
                // Scoped: one instance per request/job scope, matching the scoped DbContext and actor.
                services.AddScoped(handlerInterface, implementation);
            }
        }

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        return services;
    }

    private static bool IsHandlerInterface(Type type) =>
        type.IsGenericType
        && (type.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            || type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
