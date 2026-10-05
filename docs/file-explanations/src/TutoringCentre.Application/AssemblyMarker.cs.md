# src/TutoringCentre.Application/AssemblyMarker.cs

## Purpose

Assembly handle used by handler scanning and by architecture tests.

## Where It Fits

Application layer. `AddApplication` passes `typeof(AssemblyMarker).Assembly` to `AddCqrsHandlers`.

## Walkthrough

`internal sealed class AssemblyMarker;`. Its second job (besides tests) is to tell the scanner *which* assembly to scan without a string name - see `DependencyInjection.cs` line 18.

## Concepts Used

### Assembly scanning and reflection

#### What it means

*Reflection* lets code inspect types at runtime (`assembly.GetTypes()`, `type.GetInterfaces()`). *Assembly scanning* uses it to find classes implementing a given interface and register them automatically, so adding a new handler needs no registration line.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning](../../../PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

The marker.

#### How it works here

`services.AddCqrsHandlers(typeof(AssemblyMarker).Assembly);`

#### Why it matters here

If the type moved to another assembly the scan would silently find no handlers (the Dispatcher would then throw 'No service for type ...' at first use).

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Don't move or delete it without updating the scan call.

## Related Files

- [`src/TutoringCentre.Application/DependencyInjection.cs`](DependencyInjection.cs.md)
- [`src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs`](Common/Cqrs/HandlerRegistration.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
