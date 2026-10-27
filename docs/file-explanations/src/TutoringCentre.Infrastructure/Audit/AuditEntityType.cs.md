# `src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs`

## Purpose

Source summary (intent, not independent execution proof): The kind of record an audit entry describes. The full set audited is the allow-list in AuditPolicy (Task 31.4).

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 17 lines and 383 bytes. Source hash: `98ce8a964a000b686b23322d40fe5a868780b5a3815193212be61cc0d1b44f87`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L3): `/// <summary>The kind of record an audit entry describes. The full set audited is the allow-list in <c>AuditPolicy</c> (Task 31.4).</summary>`
- [Line 4](../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityType.cs#L4): `public enum AuditEntityType`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:98](../../../../../db/bootstrap-roles.sh#L98)
- [docs/onboarding-work/assemble.py:28](../../../../onboarding-work/assemble.py#L28)
- [docs/onboarding-work/extend_backend_purposes.py:9](../../../../onboarding-work/extend_backend_purposes.py#L9)
- [frontend/e2e/enrollment.spec.ts:19](../../../../../frontend/e2e/enrollment.spec.ts#L19)
- [frontend/e2e/full-centre-setup.spec.ts:47](../../../../../frontend/e2e/full-centre-setup.spec.ts#L47)
- [frontend/e2e/schedule.spec.ts:34](../../../../../frontend/e2e/schedule.spec.ts#L34)
- [frontend/e2e/teachers-groups.spec.ts:19](../../../../../frontend/e2e/teachers-groups.spec.ts#L19)
- [frontend/src/api/apiFetch.test.ts:79](../../../../../frontend/src/api/apiFetch.test.ts#L79)
- [frontend/src/api/apiFetch.ts:56](../../../../../frontend/src/api/apiFetch.ts#L56)
- [frontend/src/api/errorMessages.test.ts:30](../../../../../frontend/src/api/errorMessages.test.ts#L30)
- [frontend/src/api/errorMessages.ts:6](../../../../../frontend/src/api/errorMessages.ts#L6)
- [frontend/src/api/showApiError.ts:5](../../../../../frontend/src/api/showApiError.ts#L5)
- [frontend/src/components/shared/DataTable.tsx:43](../../../../../frontend/src/components/shared/DataTable.tsx#L43)
- [frontend/src/components/shared/LoadMoreButton.tsx:4](../../../../../frontend/src/components/shared/LoadMoreButton.tsx#L4)
- [frontend/src/components/shared/PhoneInput.test.tsx:6](../../../../../frontend/src/components/shared/PhoneInput.test.tsx#L6)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:44](../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L44)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:95](../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L95)
- [frontend/src/features/schedule/GroupScheduleTab.test.tsx:36](../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx#L36)
- [frontend/src/features/schedule/TimetablePage.test.tsx:142](../../../../../frontend/src/features/schedule/TimetablePage.test.tsx#L142)
- [frontend/src/features/schedule/TimetablePage.tsx:226](../../../../../frontend/src/features/schedule/TimetablePage.tsx#L226)
- 34 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Infrastructure.Audit;

/// <summary>The kind of record an audit entry describes. The full set audited is the allow-list in <c>AuditPolicy</c> (Task 31.4).</summary>
public enum AuditEntityType
{
    Subject,
    Membership,
    Centre,
    Parent,
    Student,
    StudentParent,
    Teacher,
    Group,
    Enrollment,
    ScheduleRule,
    ClassSession,
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
