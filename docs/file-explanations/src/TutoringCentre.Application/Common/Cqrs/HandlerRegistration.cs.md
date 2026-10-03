# src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs

## Purpose
`AddCqrsHandlers(services, assembly)` scans an assembly once at startup. It registers every concrete class implementing `ICommandHandler<,>` or `IQueryHandler<,>` as a **scoped** service under each closed handler interface it implements, then registers all FluentValidation validators in the assembly.

## Where it fits
Application/Common/Cqrs, `internal static`. Called by `Application/DependencyInjection.cs:18` with the Application assembly. The `Dispatcher` later resolves the handlers it registered.

## Walkthrough
- **Lines 12–13:** null guards.
- **Lines 15–17:** `assembly.GetTypes()` filtered to `{ IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }`, which excludes interfaces, abstract bases and open generics.
- **Lines 19–26:** for each type and each implemented interface where `IsHandlerInterface` is true: `services.AddScoped(handlerInterface, implementation)`. The comment explains scoped: it matches the scoped DbContext and actor.
- **Line 28:** `services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true)`, from FluentValidation's DI extensions. Validators can therefore be `internal`. Their lifetime is FluentValidation's default (scoped; that is library behaviour, not visible in this repo).
- **Lines 32–35, `IsHandlerInterface`:** the type is generic and its generic definition is `ICommandHandler<,>` or `IQueryHandler<,>`.

## Concepts used
- **Convention-based registration via reflection:** new use cases are picked up automatically.
- **Open vs closed generics:** `ICommandHandler<,>` is the open definition; `ICommandHandler<CreateCentre, Guid>` is closed.
- **Pattern matching on properties** (`type is { IsClass: true, ... }`).

## Data and control flow
Assembly types → filter → `(interface, implementation)` pairs → `IServiceCollection`.

## Configuration and environment
None.

## Gotchas and issues
- **No duplicate detection.** Two handlers for the same request register twice, and `GetRequiredService` returns the last one silently. Consider failing fast.
- `assembly.GetTypes()` can throw `ReflectionTypeLoadException` if a dependency is missing. That's unlikely for the app's own assembly.
- It's `internal`, and Application.Tests has no `InternalsVisibleTo`, so this file is **untested**: `DispatcherTests` registers handlers by hand.
- Today the Application assembly holds no handlers or validators, so this registers nothing.

## Related files
- [DependencyInjection.cs](../../DependencyInjection.cs.md)
- [Dispatcher.cs](Dispatcher.cs.md)
- [ICommandHandler.cs](ICommandHandler.cs.md)
- [IQueryHandler.cs](IQueryHandler.cs.md)
