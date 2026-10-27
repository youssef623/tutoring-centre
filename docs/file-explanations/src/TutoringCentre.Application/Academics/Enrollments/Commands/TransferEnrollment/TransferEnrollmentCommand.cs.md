# `src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Intent to move an active enrollment to another group of the same subject, guarded by the version the client last read.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 567 bytes. Source hash: `35135349f2ae327c01fbfdd9faee051e2d09e05d3b1b3f354945adc621e1fed7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentCommand.cs#L7): `public sealed record TransferEnrollmentCommand(Guid EnrollmentId, Guid TargetGroupId, uint Version)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:105](../../../../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L105)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:25](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L25)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentValidator.cs:5](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentValidator.cs#L5)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:36](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L36)
- [tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs:148](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs#L148)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs:62](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L62)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs:63](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L63)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:173](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L173)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;

/// <summary>Intent to move an active enrollment to another group of the same subject, guarded by the version the client last read.</summary>
public sealed record TransferEnrollmentCommand(Guid EnrollmentId, Guid TargetGroupId, uint Version)
    : ICommand<TransferEnrollmentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsManage;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
