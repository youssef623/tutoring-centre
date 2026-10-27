# `src/TutoringCentre.Application/Common/Security/TeacherScope.cs`

## Purpose

Source summary (intent, not independent execution proof): The three shapes a group-returning read can be limited to.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Security/TeacherScope.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 1,424 bytes. Source hash: `d7403f43b68aa3eb56157052e90f9bebbeaf4e06580e3a51e44ea463ebccdd0f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../src/TutoringCentre.Application/Common/Security/TeacherScope.cs#L4): `public enum TeacherScopeKind`
- [Line 14](../../../../../../src/TutoringCentre.Application/Common/Security/TeacherScope.cs#L14): `/// linked record must see nothing, never everything.`
- [Line 16](../../../../../../src/TutoringCentre.Application/Common/Security/TeacherScope.cs#L16): `public sealed record TeacherScope`
- [Line 21](../../../../../../src/TutoringCentre.Application/Common/Security/TeacherScope.cs#L21): `private TeacherScope(TeacherScopeKind kind, Guid? teacherId)`
- [Line 32](../../../../../../src/TutoringCentre.Application/Common/Security/TeacherScope.cs#L32): `public static TeacherScope Own(Guid teacherId) => new(TeacherScopeKind.Own, teacherId);`
- [Line 35](../../../../../../src/TutoringCentre.Application/Common/Security/TeacherScope.cs#L35): `public bool Allows(Guid teacherId) => Kind switch`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:22](../../../../../../db/bootstrap-roles.sh#L22)
- [docs/onboarding-work/assemble.py:28](../../../../../onboarding-work/assemble.py#L28)
- [docs/onboarding-work/extend_backend_purposes.py:8](../../../../../onboarding-work/extend_backend_purposes.py#L8)
- [docs/onboarding-work/extend_backend_purposes.py:9](../../../../../onboarding-work/extend_backend_purposes.py#L9)
- [frontend/e2e/auth.spec.ts:7](../../../../../../frontend/e2e/auth.spec.ts#L7)
- [frontend/e2e/enrollment.spec.ts:7](../../../../../../frontend/e2e/enrollment.spec.ts#L7)
- [frontend/e2e/first-login.spec.ts:7](../../../../../../frontend/e2e/first-login.spec.ts#L7)
- [frontend/e2e/full-centre-setup.spec.ts:7](../../../../../../frontend/e2e/full-centre-setup.spec.ts#L7)
- [frontend/e2e/schedule.spec.ts:7](../../../../../../frontend/e2e/schedule.spec.ts#L7)
- [frontend/e2e/staff-access.spec.ts:7](../../../../../../frontend/e2e/staff-access.spec.ts#L7)
- [frontend/e2e/staff-audit.spec.ts:7](../../../../../../frontend/e2e/staff-audit.spec.ts#L7)
- [frontend/e2e/students.spec.ts:7](../../../../../../frontend/e2e/students.spec.ts#L7)
- [frontend/e2e/subjects.spec.ts:7](../../../../../../frontend/e2e/subjects.spec.ts#L7)
- [frontend/e2e/teachers-groups.spec.ts:7](../../../../../../frontend/e2e/teachers-groups.spec.ts#L7)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:137](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L137)
- [frontend/src/routes/_authenticated/groups.index.test.tsx:284](../../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L284)
- [frontend/src/routes/_authenticated/settings.test.tsx:194](../../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L194)
- [frontend/src/routes/_authenticated/students.new.test.tsx:179](../../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L179)
- [frontend/src/routes/change-password.test.tsx:81](../../../../../../frontend/src/routes/change-password.test.tsx#L81)
- [smoke/playwright.config.ts:11](../../../../../../smoke/playwright.config.ts#L11)
- 50 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Security;

/// <summary>The three shapes a group-returning read can be limited to.</summary>
public enum TeacherScopeKind
{
    All,
    None,
    Own,
}

/// <summary>
/// The ownership guard's result for one request: every group, no group, or only the groups of one teacher
/// record. <see cref="None"/> is distinct from <see cref="All"/> on purpose — a Teacher-role actor with no
/// linked record must see nothing, never everything.
/// </summary>
public sealed record TeacherScope
{
    public static readonly TeacherScope All = new(TeacherScopeKind.All, null);
    public static readonly TeacherScope None = new(TeacherScopeKind.None, null);

    private TeacherScope(TeacherScopeKind kind, Guid? teacherId)
    {
        Kind = kind;
        TeacherId = teacherId;
    }

    public TeacherScopeKind Kind { get; }

    /// <summary>Set only when <see cref="Kind"/> is <see cref="TeacherScopeKind.Own"/>.</summary>
    public Guid? TeacherId { get; }

    public static TeacherScope Own(Guid teacherId) => new(TeacherScopeKind.Own, teacherId);

    /// <summary>Whether a group owned by <paramref name="teacherId"/> is within this scope.</summary>
    public bool Allows(Guid teacherId) => Kind switch
    {
        TeacherScopeKind.All => true,
        TeacherScopeKind.None => false,
        TeacherScopeKind.Own => TeacherId == teacherId,
        _ => false,
    };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
