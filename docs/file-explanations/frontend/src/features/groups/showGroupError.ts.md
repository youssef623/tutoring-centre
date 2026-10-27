# `frontend/src/features/groups/showGroupError.ts`

## Purpose

Source comment (intent): Enrolment-related refusals with their own explanation in the "groups" namespace (they start occurring on Day 48, when enrolments exist), and the two that schedules add (Day 52: reassigning to a busy teacher, archiving a scheduled group); every other error falls back to the shared translated toast.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/groups/showGroupError.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,026 bytes. Source hash: `e21b4a99d55290fc1f733439f13de5887e9fb5bdcba54fc2f4990e3995f4b8b1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { toast } from "sonner";`
- Line 2: `import i18next from "@/i18n";`
- Line 3: `import { showApiError } from "@/api/showApiError";`
- Line 4: `import type { ApiError } from "@/api/errors";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../../frontend/src/features/groups/showGroupError.ts#L18): `export function showGroupError(error: ApiError): void {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/groups/GroupEditDialogs.tsx:39](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L39)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:48](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L48)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { toast } from "sonner";
import i18next from "@/i18n";
import { showApiError } from "@/api/showApiError";
import type { ApiError } from "@/api/errors";

/**
 * Enrolment-related refusals with their own explanation in the "groups" namespace (they start occurring on
 * Day 48, when enrolments exist), and the two that schedules add (Day 52: reassigning to a busy teacher, archiving
 * a scheduled group); every other error falls back to the shared translated toast.
 */
const GroupRuleMessages: Record<string, string> = {
  "group.capacity_below_enrolled": "ruleErrors.capacityBelowEnrolled",
  "group.has_active_enrollments": "ruleErrors.hasActiveEnrollments",
  "schedule.teacher_double_booked": "ruleErrors.teacherDoubleBooked",
  "group.has_active_schedule": "ruleErrors.hasActiveSchedule",
};

export function showGroupError(error: ApiError): void {
  const key = GroupRuleMessages[error.code];
  if (key === undefined) {
    showApiError(error);
    return;
  }

  toast.error(i18next.t(key, { ns: "groups" }));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
