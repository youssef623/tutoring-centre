# `frontend/src/features/enrollments/enrollmentMessages.ts`

## Purpose

Source comment (intent): Enrollment refusals are ordinary outcomes of a busy office (two secretaries, one last seat), so they get short, specific wording in the "enrollments" namespace; anything else falls back to the shared translated message.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/enrollmentMessages.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 1,100 bytes. Source hash: `08d5dc37bb6c008cf6ab0bd63469c04c43ed58c07fc225815e8bd820867bbc52`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import i18next from "@/i18n";`
- Line 2: `import { messageFor } from "@/api/errorMessages";`
- Line 3: `import type { ApiError } from "@/api/errors";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../../frontend/src/features/enrollments/enrollmentMessages.ts#L21): `export function enrollmentMessageFor(error: ApiError): string {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:20](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L20)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:16](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L16)
- [frontend/src/features/enrollments/TransferDialog.tsx:19](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L19)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:13](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L13)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:35](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L35)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import i18next from "@/i18n";
import { messageFor } from "@/api/errorMessages";
import type { ApiError } from "@/api/errors";

/**
 * Enrollment refusals are ordinary outcomes of a busy office (two secretaries, one last seat), so they get short,
 * specific wording in the "enrollments" namespace; anything else falls back to the shared translated message.
 */
const OutcomeMessages: Record<string, string> = {
  "group.full": "outcomes.groupFull",
  "group.archived": "outcomes.groupArchived",
  "enrollment.already_enrolled": "outcomes.alreadyEnrolled",
  "enrollment.student_archived": "outcomes.studentArchived",
  "enrollment.already_ended": "outcomes.alreadyEnded",
  "enrollment.same_group": "outcomes.sameGroup",
  "enrollment.subject_mismatch": "outcomes.subjectMismatch",
  "concurrency.stale": "outcomes.stale",
  "student.has_active_enrollments": "outcomes.studentHasActiveEnrollments",
};

export function enrollmentMessageFor(error: ApiError): string {
  const key = OutcomeMessages[error.code];
  return key === undefined ? messageFor(error) : i18next.t(key, { ns: "enrollments" });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
