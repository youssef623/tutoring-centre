# `src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to cancel one future session (a holiday, a sick teacher), guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 497 bytes. Source hash: `fa9910ab6a0f18d54f0cad46ecd9429ef6dc1e641b25514ca9afdee699a91c78`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionCommand.cs#L7): `public sealed record CancelSessionCommand(Guid SessionId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:127](../../../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L127)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs:14](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/CancelSession/CancelSessionValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs:37](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs#L37)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs:51](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs#L51)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs:151](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs#L151)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;

/// <summary>Intent to cancel one future session (a holiday, a sick teacher), guarded by the version the client last read.</summary>
public sealed record CancelSessionCommand(Guid SessionId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ScheduleManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
