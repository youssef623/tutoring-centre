# `src/TutoringCentre.Domain/Common/Result.cs`

## Purpose

Source summary (intent, not independent execution proof): The outcome of an operation that can fail for an expected business reason.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Common/Result.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 70 lines and 2,405 bytes. Source hash: `0c0e79416f52e29b4267468790df29e5b5dbb4d455c65ef98494c72dd6a6d640`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../src/TutoringCentre.Domain/Common/Result.cs#L6): `public class Result`
- [Line 8](../../../../../src/TutoringCentre.Domain/Common/Result.cs#L8): `protected Result(bool isSuccess, Error? error)`
- [Line 32](../../../../../src/TutoringCentre.Domain/Common/Result.cs#L32): `public static Result Success() => new(isSuccess: true, error: null);`
- [Line 35](../../../../../src/TutoringCentre.Domain/Common/Result.cs#L35): `public static Result Failure(Error error) => new(isSuccess: false, error);`
- [Line 43](../../../../../src/TutoringCentre.Domain/Common/Result.cs#L43): `public sealed class Result<T> : Result`
- [Line 47](../../../../../src/TutoringCentre.Domain/Common/Result.cs#L47): `private Result(T value)`
- [Line 53](../../../../../src/TutoringCentre.Domain/Common/Result.cs#L53): `private Result(Error error)`
- [Line 66](../../../../../src/TutoringCentre.Domain/Common/Result.cs#L66): `public static Result<T> Success(T value) => new(value);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:28](../../../../onboarding-work/extend_backend_purposes.py#L28)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:493](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L493)
- [src/TutoringCentre.Api/Http/ResultHttpExtensions.cs:13](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs#L13)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:27](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L27)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:27](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L27)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs:18](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs#L18)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs:17](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/GetGroupRoster/GetGroupRosterHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs:17](../../../../../src/TutoringCentre.Application/Academics/Enrollments/Queries/ListStudentEnrollments/ListStudentEnrollmentsHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs:24](../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ArchiveGroup/ArchiveGroupHandler.cs#L24)
- [src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs:26](../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/AssignGroupTeacher/AssignGroupTeacherHandler.cs#L26)
- [src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs:15](../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/ChangeGroupCapacity/ChangeGroupCapacityHandler.cs#L15)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:24](../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L24)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs:14](../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RenameGroup/RenameGroupHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs:18](../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L18)
- [src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs:16](../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/GetGroup/GetGroupHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs:13](../../../../../src/TutoringCentre.Application/Academics/Groups/Queries/ListGroups/ListGroupsHandler.cs#L13)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:14](../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs:16](../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs#L16)
- [src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs:11](../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs:11](../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs#L11)
- 120 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Domain.Common;

/// <summary>The outcome of an operation that can fail for an expected business reason.</summary>
public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        // The only two legal states: success without an error, failure with one.
        if (isSuccess && error is not null)
        {
            throw new ArgumentException("A successful result cannot carry an error.", nameof(error));
        }

        if (!isSuccess && error is null)
        {
            throw new ArgumentNullException(nameof(error), "A failed result must carry an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    /// <summary>The failure's error; null when <see cref="IsSuccess"/> is true.</summary>
    public Error? Error { get; }

    public static Result Success() => new(isSuccess: true, error: null);

    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static Result Failure(Error error) => new(isSuccess: false, error);
}

/// <summary>The outcome of an operation that produces a <typeparamref name="T"/> or fails for an expected business reason.</summary>
[SuppressMessage(
    "Design",
    "CA1000:Do not declare static members on generic types",
    Justification = "Result<T>.Success/Failure are the agreed factory API; call sites always name the type.")]
public sealed class Result<T> : Result
{
    private readonly T _value;

    private Result(T value)
        : base(isSuccess: true, error: null)
    {
        _value = value;
    }

    private Result(Error error)
        : base(isSuccess: false, error)
    {
        _value = default!;
    }

    /// <summary>The produced value.</summary>
    /// <exception cref="InvalidOperationException">The result is a failure — reading it is a bug.</exception>
    public T Value => IsSuccess
        ? _value
        : throw new InvalidOperationException(
            $"Cannot read Value of a failed result (error code '{Error?.Code}'). Check IsSuccess first.");

    public static Result<T> Success(T value) => new(value);

    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static new Result<T> Failure(Error error) => new(error);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
