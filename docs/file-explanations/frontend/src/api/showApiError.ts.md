# `frontend/src/api/showApiError.ts`

## Purpose

Source comment (intent): Shows a toast with the translated message; the correlation reference lets a user report an error that can be found in the logs.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/api/showApiError.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 35 lines and 1,295 bytes. Source hash: `3ecec1d6a25a0a971f8744d115a0970ad6610cc422fcb04b21d702d0d304eeaa`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { toast } from "sonner";`
- Line 2: `import { messageFor } from "./errorMessages";`
- Line 3: `import type { ApiError } from "./errors";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../frontend/src/api/showApiError.ts#L6): `export function showApiError(error: ApiError): void {`
- [Line 17](../../../../../frontend/src/api/showApiError.ts#L17): `export function applyFieldErrors<TField extends string>(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/api/showApiError.test.ts:3](../../../../../frontend/src/api/showApiError.test.ts#L3)
- [frontend/src/components/shared/useParentLookup.ts:3](../../../../../frontend/src/components/shared/useParentLookup.ts#L3)
- [frontend/src/features/groups/CreateGroupDialog.tsx:10](../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L10)
- [frontend/src/features/groups/showGroupError.ts:3](../../../../../frontend/src/features/groups/showGroupError.ts#L3)
- [frontend/src/features/parents/ParentEditDialog.tsx:8](../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L8)
- [frontend/src/features/staff/ChangeRoleDialog.tsx:7](../../../../../frontend/src/features/staff/ChangeRoleDialog.tsx#L7)
- [frontend/src/features/staff/StaffFormDialog.tsx:9](../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L9)
- [frontend/src/features/staff/StaffList.tsx:8](../../../../../frontend/src/features/staff/StaffList.tsx#L8)
- [frontend/src/features/students/AddParentDialog.tsx:9](../../../../../frontend/src/features/students/AddParentDialog.tsx#L9)
- [frontend/src/features/students/ParentsCard.tsx:8](../../../../../frontend/src/features/students/ParentsCard.tsx#L8)
- [frontend/src/features/students/RegisterStudentForm.tsx:9](../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L9)
- [frontend/src/features/students/StudentEditDialog.tsx:8](../../../../../frontend/src/features/students/StudentEditDialog.tsx#L8)
- [frontend/src/features/subjects/SubjectFormDialog.tsx:9](../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx#L9)
- [frontend/src/features/subjects/SubjectsList.tsx:8](../../../../../frontend/src/features/subjects/SubjectsList.tsx#L8)
- [frontend/src/features/teachers/TeacherAccountDialog.tsx:9](../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L9)
- [frontend/src/features/teachers/TeacherFormDialog.tsx:9](../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L9)
- [frontend/src/features/teachers/TeachersList.tsx:8](../../../../../frontend/src/features/teachers/TeachersList.tsx#L8)
- [frontend/src/routes/_authenticated/settings.tsx:10](../../../../../frontend/src/routes/_authenticated/settings.tsx#L10)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:9](../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L9)
- [frontend/src/routes/change-password.tsx:13](../../../../../frontend/src/routes/change-password.tsx#L13)
- 2 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { toast } from "sonner";
import { messageFor } from "./errorMessages";
import type { ApiError } from "./errors";

/** Shows a toast with the translated message; the correlation reference lets a user report an error that can be found in the logs. */
export function showApiError(error: ApiError): void {
  const options =
    error.correlationId === undefined ? {} : { description: `Reference: ${error.correlationId}` };

  toast.error(messageFor(error), options);
}

/**
 * Maps server field errors onto form fields. Only fields the form knows about are applied (unknown keys are ignored);
 * the first message per field is used. Compatible with React Hook Form's setError (Day 17).
 */
export function applyFieldErrors<TField extends string>(
  error: ApiError,
  setError: (field: TField, option: { type: string; message: string }) => void,
  knownFields: readonly TField[],
): void {
  if (error.fieldErrors === undefined) {
    return;
  }

  const isKnownField = (field: string): field is TField =>
    (knownFields as readonly string[]).includes(field);

  for (const [field, messages] of Object.entries(error.fieldErrors)) {
    const message = messages.at(0);
    if (message !== undefined && isKnownField(field)) {
      setError(field, { type: "server", message });
    }
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
