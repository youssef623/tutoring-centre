# `src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Orchestrates: scoped lookup → the end transition on the entity. The row is kept with its end time and reason. No group lock: freeing a seat can never break capacity, so only the enrollment's own row version guards against two people acting on the same row.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 1,442 bytes. Source hash: `e015461759977c90e1ae410895a648f7931871c1ecf0f56fb15526bece4bef49`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Ports;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs#L15): `internal sealed class WithdrawEnrollmentHandler(IClock clock, IEnrollmentRepository enrollments)`
- [Line 18](../../../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs#L18): `public async Task<Result<Unit>> HandleAsync(WithdrawEnrollmentCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:157](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L157)
- [tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs:15](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;

/// <summary>
/// Orchestrates: scoped lookup → the end transition on the entity. The row is kept with its end time and reason.
/// No group lock: freeing a seat can never break capacity, so only the enrollment's own row version guards
/// against two people acting on the same row.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class WithdrawEnrollmentHandler(IClock clock, IEnrollmentRepository enrollments)
    : ICommandHandler<WithdrawEnrollmentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(WithdrawEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var enrollment = await enrollments.GetByIdAsync(command.EnrollmentId, command.Version, cancellationToken);
        if (enrollment is null)
        {
            return Result<Unit>.Failure(EnrollmentErrors.NotFound());
        }

        var ended = enrollment.End(clock.UtcNow, EnrollmentEndReason.Withdrawn);
        return ended.IsFailure ? Result<Unit>.Failure(ended.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
