# `src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs`

## Purpose

The scoped provider converts actor identity into All, Own, or None group ownership scope and caches that decision once per request. Teacher roles resolve through an active linked academic teacher; missing linkage fails closed. System and unrestricted staff roles get All while unknown actors never default to unrestricted data.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 49 lines and 2,105 bytes. Source hash: `71f836b1ea947f39e37701ec25045978b15a462aa6928e02501f3fa9128cd8f5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Academics.Teachers;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs#L12): `/// <see cref="TeacherScope.None"/> when none is linked or the linked record is archived (that lookup only`
- [Line 18](../../../../../src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs#L18): `internal sealed class TeacherScopeProvider(ICurrentActor currentActor, ITeacherReadService teachers) : ITeacherScopeProvider`
- [Line 22](../../../../../src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs#L22): `public async Task<TeacherScope> GetAsync(CancellationToken ct)`
- [Line 34](../../../../../src/TutoringCentre.Infrastructure/Identity/TeacherScopeProvider.cs#L34): `private async Task<TeacherScope> ResolveAsync(CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:70](../../../../../db/bootstrap-roles.sh#L70)
- [docs/onboarding-work/assemble.py:14](../../../../onboarding-work/assemble.py#L14)
- [docs/onboarding-work/extend_backend_purposes.py:8](../../../../onboarding-work/extend_backend_purposes.py#L8)
- [docs/onboarding-work/file_guides.py:57](../../../../onboarding-work/file_guides.py#L57)
- [docs/onboarding-work/inspect-tests.py:25](../../../../onboarding-work/inspect-tests.py#L25)
- [docs/onboarding-work/read-generated-models.py:36](../../../../onboarding-work/read-generated-models.py#L36)
- [docs/onboarding-work/reconcile_backend_ledger.py:17](../../../../onboarding-work/reconcile_backend_ledger.py#L17)
- [docs/onboarding-work/validate_docs.py:15](../../../../onboarding-work/validate_docs.py#L15)
- [frontend/e2e/enrollment.spec.ts:96](../../../../../frontend/e2e/enrollment.spec.ts#L96)
- [frontend/e2e/first-login.spec.ts:23](../../../../../frontend/e2e/first-login.spec.ts#L23)
- [frontend/e2e/schedule.spec.ts:97](../../../../../frontend/e2e/schedule.spec.ts#L97)
- [frontend/src/api/apiFetch.ts:56](../../../../../frontend/src/api/apiFetch.ts#L56)
- [frontend/src/api/csrf.ts:24](../../../../../frontend/src/api/csrf.ts#L24)
- [frontend/src/api/errors.ts:14](../../../../../frontend/src/api/errors.ts#L14)
- [frontend/src/api/problemDetails.test.ts:27](../../../../../frontend/src/api/problemDetails.test.ts#L27)
- [frontend/src/api/problemDetails.ts:17](../../../../../frontend/src/api/problemDetails.ts#L17)
- [frontend/src/api/showApiError.test.ts:32](../../../../../frontend/src/api/showApiError.test.ts#L32)
- [frontend/src/api/showApiError.ts:26](../../../../../frontend/src/api/showApiError.ts#L26)
- [frontend/src/app/queryClient.ts:50](../../../../../frontend/src/app/queryClient.ts#L50)
- [frontend/src/components/shared/LoadMoreButton.test.tsx:6](../../../../../frontend/src/components/shared/LoadMoreButton.test.tsx#L6)
- 237 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// Resolves the ownership guard for group-returning requests. A system actor or a role not limited to its
/// own groups (<see cref="RoleScopes.IsLimitedToOwnGroups"/>) gets <see cref="TeacherScope.All"/>; a limited
/// role resolves through <see cref="ITeacherReadService.FindIdByUserAsync"/> to its own teacher record, or
/// <see cref="TeacherScope.None"/> when none is linked or the linked record is archived (that lookup only
/// ever returns an active teacher's ID). Any other actor — anonymous, or staff with no selected role —
/// also gets <see cref="TeacherScope.None"/>: fails closed, never defaults to All on an unknown case.
/// Registered scoped, so the result is computed at most once per request.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TeacherScopeProvider(ICurrentActor currentActor, ITeacherReadService teachers) : ITeacherScopeProvider
{
    private TeacherScope? _cached;

    public async Task<TeacherScope> GetAsync(CancellationToken ct)
    {
        if (_cached is { } cached)
        {
            return cached;
        }

        var resolved = await ResolveAsync(ct);
        _cached = resolved;
        return resolved;
    }

    private async Task<TeacherScope> ResolveAsync(CancellationToken ct)
    {
        if (currentActor.Actor is not StaffActor staffActor || staffActor.Role is not { } role)
        {
            return currentActor.Actor is SystemActor ? TeacherScope.All : TeacherScope.None;
        }

        if (!RoleScopes.IsLimitedToOwnGroups(role))
        {
            return TeacherScope.All;
        }

        var teacherId = await teachers.FindIdByUserAsync(staffActor.UserId, ct);
        return teacherId is { } id ? TeacherScope.Own(id) : TeacherScope.None;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
