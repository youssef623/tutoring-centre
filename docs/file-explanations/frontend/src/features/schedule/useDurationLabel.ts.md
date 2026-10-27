# `frontend/src/features/schedule/useDurationLabel.ts`

## Purpose

Source comment (intent): A length in minutes as people say it: "45 minutes", "1 hour 30 minutes", "2 hours".

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/useDurationLabel.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 415 bytes. Source hash: `66e9d4bc8e21e080e45aad0f3d1fd3132db21705315c12592b5b7a1b66826858`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../frontend/src/features/schedule/useDurationLabel.ts#L4): `export function useDurationLabel() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:33](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L33)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { useTranslation } from "react-i18next";

/** A length in minutes as people say it: "45 minutes", "1 hour 30 minutes", "2 hours". */
export function useDurationLabel() {
  const { t } = useTranslation("schedule");

  return (minutes: number) =>
    minutes < 60
      ? t("durations.minutes", { minutes })
      : t("durations.hoursAndMinutes", { hours: Math.floor(minutes / 60), minutes: minutes % 60 });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
