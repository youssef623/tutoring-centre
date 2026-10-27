# `frontend/src/features/session/useCentreTimeZone.ts`

## Purpose

Source comment (intent): The active centre's IANA zone, from the signed-in session's membership (Day 53), or null before a centre is selected. Every role has it, unlike the centre settings, so any page can work out "today" in the centre's calendar. Never falls back to the browser's zone.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/useCentreTimeZone.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 521 bytes. Source hash: `bc8d4b7e99371cd1087b34fd4855e0ee4ae3275ba8f5e236475342f32a4cadd5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useSession } from "./useSession";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/features/session/useCentreTimeZone.ts#L8): `export function useCentreTimeZone(): string | null {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/GroupScheduleTab.tsx:20](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L20)
- [frontend/src/features/schedule/TimetablePage.tsx:24](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L24)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { useSession } from "./useSession";

/**
 * The active centre's IANA zone, from the signed-in session's membership (Day 53), or null before a centre is
 * selected. Every role has it, unlike the centre settings, so any page can work out "today" in the centre's
 * calendar. Never falls back to the browser's zone.
 */
export function useCentreTimeZone(): string | null {
  const { me } = useSession();
  return me?.memberships.find((membership) => membership.centreId === me.activeCentreId)?.timeZoneId ?? null;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
