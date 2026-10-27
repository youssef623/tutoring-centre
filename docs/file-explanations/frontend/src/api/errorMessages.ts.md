# `frontend/src/api/errorMessages.ts`

## Purpose

Source comment (intent): The one function every form and toast uses to turn an ApiError into user-facing text.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/api/errorMessages.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 19 lines and 573 bytes. Source hash: `bfcd8208948d4cac23b9767e6981efb475cabe6fcc831b72d097cf2e41e5032f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import i18next from "@/i18n";`
- Line 2: `import type { ApiError } from "./errors";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../frontend/src/api/errorMessages.ts#L6): `/** The one function every form and toast uses to turn an ApiError into user-facing text. */`
- [Line 7](../../../../../frontend/src/api/errorMessages.ts#L7): `export function messageFor(error: ApiError): string {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:36](../../../../onboarding-work/extend_backend_purposes.py#L36)
- [docs/onboarding-work/file_guides.py:82](../../../../onboarding-work/file_guides.py#L82)
- [docs/onboarding-work/read-generated-models.py:36](../../../../onboarding-work/read-generated-models.py#L36)
- [docs/onboarding-work/read_frontend.py:9](../../../../onboarding-work/read_frontend.py#L9)
- [docs/onboarding-work/reconcile_backend_ledger.py:17](../../../../onboarding-work/reconcile_backend_ledger.py#L17)
- [frontend/src/api/errorMessages.test.ts:3](../../../../../frontend/src/api/errorMessages.test.ts#L3)
- [frontend/src/api/errors.ts:14](../../../../../frontend/src/api/errors.ts#L14)
- [frontend/src/api/fieldErrors.ts:2](../../../../../frontend/src/api/fieldErrors.ts#L2)
- [frontend/src/api/showApiError.ts:2](../../../../../frontend/src/api/showApiError.ts#L2)
- [frontend/src/app/queryClient.ts:73](../../../../../frontend/src/app/queryClient.ts#L73)
- [frontend/src/components/shared/FilterBar.tsx:3](../../../../../frontend/src/components/shared/FilterBar.tsx#L3)
- [frontend/src/features/audit/AuditLog.tsx:6](../../../../../frontend/src/features/audit/AuditLog.tsx#L6)
- [frontend/src/features/dashboard/DashboardOverview.tsx:5](../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L5)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:6](../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L6)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:71](../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L71)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:5](../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L5)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:6](../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L6)
- [frontend/src/features/enrollments/StudentPicker.tsx:6](../../../../../frontend/src/features/enrollments/StudentPicker.tsx#L6)
- [frontend/src/features/enrollments/TransferDialog.tsx:6](../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L6)
- [frontend/src/features/enrollments/enrollmentMessages.ts:2](../../../../../frontend/src/features/enrollments/enrollmentMessages.ts#L2)
- 24 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import i18next from "@/i18n";
import type { ApiError } from "./errors";

const Namespace = "errors";

/** The one function every form and toast uses to turn an ApiError into user-facing text. */
export function messageFor(error: ApiError): string {
  const byCode = i18next.t(`byCode.${error.code}`, { ns: Namespace, defaultValue: "" });
  if (byCode !== "") {
    return byCode;
  }

  const byKind = i18next.t(`byKind.${error.kind}`, { ns: Namespace, defaultValue: "" });
  if (byKind !== "") {
    return byKind;
  }

  return i18next.t("generic", { ns: Namespace });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
