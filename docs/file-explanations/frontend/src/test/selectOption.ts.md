# `frontend/src/test/selectOption.ts`

## Purpose

Source comment (intent): Opens a Base UI `Select` by its trigger's accessible name and picks the option with the given name. Base UI's `Select.Item` only commits a mouse click when it was preceded by a `pointerdown` on the same item (see `allowMouseSelectionRef` in its source) — a real click is always `pointerdown` then `click`, but `fireEvent.click` alone dispatches only the `click` event, so the selection is silently ignored without this.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/test/selectOption.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 810 bytes. Source hash: `6fa8256cecbe8140437c829305819c6e986c4e2231ffcdd492d25199d9642cde`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen } from "@testing-library/react";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../frontend/src/test/selectOption.ts#L11): `export async function selectOption(triggerName: string, optionName: string): Promise<void> {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:8](../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L8)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:12](../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L12)
- [frontend/src/routes/_authenticated/audit.test.tsx:5](../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L5)
- [frontend/src/routes/_authenticated/groups.index.test.tsx:7](../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L7)
- [frontend/src/routes/_authenticated/settings.test.tsx:5](../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L5)
- [frontend/src/routes/_authenticated/staff.test.tsx:5](../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L5)
- [frontend/src/routes/_authenticated/students.new.test.tsx:5](../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L5)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { fireEvent, screen } from "@testing-library/react";

/**
 * Opens a Base UI `Select` by its trigger's accessible name and picks the option with the given name.
 *
 * Base UI's `Select.Item` only commits a mouse click when it was preceded by a `pointerdown` on the same
 * item (see `allowMouseSelectionRef` in its source) — a real click is always `pointerdown` then `click`,
 * but `fireEvent.click` alone dispatches only the `click` event, so the selection is silently ignored
 * without this.
 */
export async function selectOption(triggerName: string, optionName: string): Promise<void> {
  fireEvent.click(screen.getByRole("combobox", { name: triggerName }));
  const option = await screen.findByRole("option", { name: optionName });
  fireEvent.pointerDown(option);
  fireEvent.click(option);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
