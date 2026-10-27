# `frontend/src/features/schedule/scheduleMessages.ts`

## Purpose

Source comment (intent): Schedule refusals are ordinary outcomes of a busy office (a teacher already booked, a session someone else cancelled first), so they get short, specific wording in the "schedule" namespace; anything else falls back to the shared translated message.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/scheduleMessages.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,494 bytes. Source hash: `d78c460ff4b7d7e97eb2642bddd5ff979b88935a44c86b4e25ef60fa2fdda640`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import i18next from "@/i18n";`
- Line 2: `import { messageFor } from "@/api/errorMessages";`
- Line 3: `import type { ApiError } from "@/api/errors";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../../frontend/src/features/schedule/scheduleMessages.ts#L24): `export function scheduleMessageFor(error: ApiError): string {`
- [Line 30](../../../../../../frontend/src/features/schedule/scheduleMessages.ts#L30): `export function isDoubleBooking(error: ApiError): boolean {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:23](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L23)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:15](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L15)
- [frontend/src/features/schedule/EndScheduleRuleDialog.tsx:18](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx#L18)
- [frontend/src/features/schedule/ExtendScheduleDialog.tsx:17](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import i18next from "@/i18n";
import { messageFor } from "@/api/errorMessages";
import type { ApiError } from "@/api/errors";

/**
 * Schedule refusals are ordinary outcomes of a busy office (a teacher already booked, a session someone else
 * cancelled first), so they get short, specific wording in the "schedule" namespace; anything else falls back to
 * the shared translated message.
 */
const OutcomeMessages: Record<string, string> = {
  "schedule.teacher_double_booked": "outcomes.teacherDoubleBooked",
  "schedule.group_double_booked": "outcomes.groupDoubleBooked",
  "group.archived": "outcomes.groupArchived",
  "schedule.range_invalid": "outcomes.rangeInvalid",
  "schedule.rule_already_ended": "outcomes.ruleAlreadyEnded",
  "schedule.rule_end_invalid": "outcomes.ruleEndInvalid",
  "schedule.rule_not_found": "outcomes.ruleNotFound",
  "schedule.session_already_cancelled": "outcomes.sessionAlreadyCancelled",
  "schedule.session_in_past": "outcomes.sessionInPast",
  "schedule.session_not_found": "outcomes.sessionNotFound",
  "concurrency.stale": "outcomes.stale",
};

export function scheduleMessageFor(error: ApiError): string {
  const key = OutcomeMessages[error.code];
  return key === undefined ? messageFor(error) : i18next.t(key, { ns: "schedule" });
}

/** The two exclusion-constraint conflicts. */
export function isDoubleBooking(error: ApiError): boolean {
  return error.code === "schedule.teacher_double_booked" || error.code === "schedule.group_double_booked";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
