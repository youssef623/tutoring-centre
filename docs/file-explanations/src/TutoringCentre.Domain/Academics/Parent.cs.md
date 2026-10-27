# `src/TutoringCentre.Domain/Academics/Parent.cs`

## Purpose

Source summary (intent, not independent execution proof): An adult the centre contacts about one or more students. A parent has no status: it is never archived or deleted. The only way to obtain one is `Create`, which enforces its invariants.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 91 lines and 3,050 bytes. Source hash: `4876ec6d491437e8b09ed12fea4a33de589d6bd1cde56ec005b585a92fc5c02b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.RegularExpressions;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L10): `public sealed partial class Parent : Entity, ITenantOwned`
- [Line 15](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L15): `private Parent(Guid centreId, string fullName, PhoneNumber phone)`
- [Line 23](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L23): `private Parent()`
- [Line 40](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L40): `public static Result<Parent> Create(Guid centreId, string fullName, PhoneNumber phone)`
- [Line 56](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L56): `public Result Update(string fullName, PhoneNumber phone)`
- [Line 70](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L70): `private void SetName(string cleanedName)`
- [Line 76](../../../../../src/TutoringCentre.Domain/Academics/Parent.cs#L76): `private static Result<string> ValidateName(string? fullName)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/e2e/students.spec.ts:64](../../../../../frontend/e2e/students.spec.ts#L64)
- [frontend/src/features/parents/updateParentSchema.ts:3](../../../../../frontend/src/features/parents/updateParentSchema.ts#L3)
- [frontend/src/routes/_authenticated/parents.$parentId.test.tsx:76](../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx#L76)
- [frontend/src/routes/_authenticated/students.$studentId.test.tsx:275](../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L275)
- [frontend/src/routes/_authenticated/students.new.test.tsx:37](../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L37)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:265](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L265)
- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:121](../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L121)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:31](../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L31)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentValidator.cs:13](../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentValidator.cs#L13)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentValidator.cs:14](../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentValidator.cs#L14)
- [src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs:26](../../../../../src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs#L26)
- [src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs:18](../../../../../src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs#L18)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs:13](../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentCommand.cs#L13)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:38](../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L38)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs:21](../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentValidator.cs#L21)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs:9](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L9)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs:16](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L16)
- [src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs:37](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditPolicy.cs#L37)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs:8](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/ParentConfiguration.cs#L8)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs:46](../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/StudentParentLinkConfiguration.cs#L46)
- 38 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// An adult the centre contacts about one or more students. A parent has no status: it is never archived or
/// deleted. The only way to obtain one is <see cref="Create"/>, which enforces its invariants.
/// </summary>
public sealed partial class Parent : Entity, ITenantOwned
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 120;

    private Parent(Guid centreId, string fullName, PhoneNumber phone)
    {
        CentreId = centreId;
        Phone = phone;
        SetName(fullName);
    }

    // For EF Core materialisation (Day 38) only; EF then sets every property from the row.
    private Parent()
    {
        FullName = string.Empty;
        SearchName = string.Empty;
        Phone = null!;
    }

    /// <summary>The centre this parent belongs to; set once at creation and never changeable.</summary>
    public Guid CentreId { get; private set; }

    public string FullName { get; private set; }

    /// <summary>Normalized form of <see cref="FullName"/> for matching, never shown to users.</summary>
    public string SearchName { get; private set; }

    public PhoneNumber Phone { get; private set; }

    public static Result<Parent> Create(Guid centreId, string fullName, PhoneNumber phone)
    {
        if (centreId == Guid.Empty)
        {
            return Result<Parent>.Failure(Error.Validation("parent.centre_required", "A centre is required."));
        }

        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result<Parent>.Failure(cleanedName.Error!);
        }

        return Result<Parent>.Success(new Parent(centreId, cleanedName.Value, phone));
    }

    public Result Update(string fullName, PhoneNumber phone)
    {
        var cleanedName = ValidateName(fullName);
        if (cleanedName.IsFailure)
        {
            return Result.Failure(cleanedName.Error!);
        }

        SetName(cleanedName.Value);
        Phone = phone;
        return Result.Success();
    }

    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(FullName), nameof(SearchName))]
    private void SetName(string cleanedName)
    {
        FullName = cleanedName;
        SearchName = SearchText.Normalize(cleanedName);
    }

    private static Result<string> ValidateName(string? fullName)
    {
        var cleaned = WhitespacePattern().Replace((fullName ?? string.Empty).Trim(), " ");
        if (cleaned.Length < NameMinLength || cleaned.Length > NameMaxLength)
        {
            var fields = new Dictionary<string, string[]> { ["fullName"] = ["Parent name must be 2-120 characters."] };
            return Result<string>.Failure(
                Error.Validation("parent.name_invalid", "Parent name must be 2-120 characters.", fields));
        }

        return Result<string>.Success(cleaned);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
```

The excerpt stops at line 90; use the original file for the remaining 1 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
