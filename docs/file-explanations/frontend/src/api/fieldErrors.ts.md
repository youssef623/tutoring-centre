# `frontend/src/api/fieldErrors.ts`

## Purpose

Source comment (intent): True when errors.json has a message for this exact code (not just its kind or the generic fallback).

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/api/fieldErrors.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 45 lines and 1,930 bytes. Source hash: `ca94866e6b1428f3c1fb5f10770ed99bbf2050098dcc584daf3e48698d1232c8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import i18next from "@/i18n";`
- Line 2: `import { messageFor } from "./errorMessages";`
- Line 3: `import type { ApiError } from "./errors";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../frontend/src/api/fieldErrors.ts#L8): `function hasCodeMessage(error: ApiError): boolean {`
- [Line 19](../../../../../frontend/src/api/fieldErrors.ts#L19): `export function applyTranslatedFieldErrors<TField extends string>(`
- [Line 43](../../../../../frontend/src/api/fieldErrors.ts#L43): `export function setCodedFieldError<TField extends string>(error: ApiError, setError: SetFieldError<TField>, field: TField): void {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/groups/CreateGroupDialog.tsx:9](../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L9)
- [frontend/src/features/groups/GroupEditDialogs.tsx:10](../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L10)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:7](../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L7)
- [frontend/src/features/teachers/TeacherFormDialog.tsx:8](../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L8)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import i18next from "@/i18n";
import { messageFor } from "./errorMessages";
import type { ApiError } from "./errors";

type SetFieldError<TField extends string> = (field: TField, option: { type: string; message: string }) => void;

/** True when errors.json has a message for this exact code (not just its kind or the generic fallback). */
function hasCodeMessage(error: ApiError): boolean {
  return i18next.t(`byCode.${error.code}`, { ns: "errors", defaultValue: "" }) !== "";
}

/**
 * Like `applyFieldErrors`, but the message under each field is the translated message for the error's code
 * (e.g. `group.name_taken`, `group.capacity_invalid`) whenever errors.json has one, so an Arabic session sees
 * Arabic under the field; the server's own field text is only the fallback. The `type` stays "server", so a
 * form renders `message` as-is rather than treating it as an i18n key.
 * @returns true when at least one known field received an error.
 */
export function applyTranslatedFieldErrors<TField extends string>(
  error: ApiError,
  setError: SetFieldError<TField>,
  knownFields: readonly TField[],
): boolean {
  if (error.fieldErrors === undefined) {
    return false;
  }

  const translated = hasCodeMessage(error) ? messageFor(error) : undefined;
  let applied = false;

  for (const [field, messages] of Object.entries(error.fieldErrors)) {
    const message = translated ?? messages.at(0);
    if (message !== undefined && (knownFields as readonly string[]).includes(field)) {
      setError(field as TField, { type: "server", message });
      applied = true;
    }
  }

  return applied;
}

/** Puts the translated message for a coded refusal (one the server reports without a field) under a chosen field. */
export function setCodedFieldError<TField extends string>(error: ApiError, setError: SetFieldError<TField>, field: TField): void {
  setError(field, { type: "server", message: messageFor(error) });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
