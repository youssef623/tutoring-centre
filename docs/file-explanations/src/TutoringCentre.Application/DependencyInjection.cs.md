# src/TutoringCentre.Application/DependencyInjection.cs

## Purpose
`AddApplication(this IServiceCollection)` is the Application layer's single DI entry point. It registers the per-scope actor context, every command and query handler and validator in the assembly, and the `Dispatcher`.

## Where it fits
Application layer. Called once from `src/TutoringCentre.Api/Program.cs:16`. It depends on `CurrentActorContext`, `ICurrentActor`, `HandlerRegistration` and `Dispatcher`.

## Walkthrough
- **Line 12:** null guard.
- **Lines 14–16:** `AddScoped<CurrentActorContext>()`, plus `AddScoped<ICurrentActor>(p => p.GetRequiredService<CurrentActorContext>())`. Both service types resolve to the **same** scoped instance: edge code sets it through the concrete type, and handlers read it through the read-only interface.
- **Line 18:** `services.AddCqrsHandlers(typeof(AssemblyMarker).Assembly)` registers handlers (scoped) and FluentValidation validators.
- **Line 19:** `AddScoped<Dispatcher>()`.
- **Line 21:** returns `services` so calls can chain: `AddApplication().AddInfrastructure(...)`.

## Concepts used
- **Per-layer DI extension method:** `Program.cs` never needs to know a layer's internals.
- **Forwarding registration:** one instance exposed as two service types (`ICurrentActor` → `CurrentActorContext`).
- **Scoped lifetime:** one instance per HTTP request or job scope.

## Data and control flow
`Program.cs` → `AddApplication` → service collection entries.

## Configuration and environment
None.

## Gotchas and issues
- Nothing in production calls `CurrentActorContext.Set` yet, so every scope's actor is `AnonymousActor`.
- No handlers or validators exist in the Application assembly yet, so the scan registers nothing today.

## Related files
- [HandlerRegistration.cs](Common/Cqrs/HandlerRegistration.cs.md)
- [Dispatcher.cs](Common/Cqrs/Dispatcher.cs.md)
- [CurrentActorContext.cs](Common/Security/CurrentActorContext.cs.md)
- [Program.cs](../TutoringCentre.Api/Program.cs.md)
