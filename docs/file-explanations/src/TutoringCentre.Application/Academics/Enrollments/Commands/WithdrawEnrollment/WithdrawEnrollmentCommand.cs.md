# `src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to end an enrollment and free its seat, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 500 bytes. Source hash: `c29b7b9cea383fec98418c139ead0a090eda965f216340935d07957afc7fb7fe`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentCommand.cs#L7): `public sealed record WithdrawEnrollmentCommand(Guid EnrollmentId, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:98](../../../../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L98)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs:16](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:35](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L35)
- [tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs:17](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs:50](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L50)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:191](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L191)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;

/// <summary>Intent to end an enrollment and free its seat, guarded by the version the client last read.</summary>
public sealed record WithdrawEnrollmentCommand(Guid EnrollmentId, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
