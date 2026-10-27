# `frontend/src/features/enrollments/GroupChoiceList.tsx`

## Purpose

Source comment (intent): A radio list of groups with their teacher and seats, used to choose a transfer target or a group to enroll in. Full groups, and groups the caller marks unavailable, are shown but disabled. The seat numbers are the server's and only advisory: the request is still re-checked, and a 409 is shown by the caller.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/GroupChoiceList.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 66 lines and 2,488 bytes. Source hash: `d1b03cd766e36c5261524451ccd2a7753e747ad3690e6e5a913a3b5bc8149c89`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import type { GroupDto } from "@/api/generated/tutoring-centre";`
- Line 3: `import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";`
- Line 4: `import { SeatMeter } from "@/features/enrollments/SeatMeter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../frontend/src/features/enrollments/GroupChoiceList.tsx#L11): `export function GroupChoiceList({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:21](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L21)
- [frontend/src/features/enrollments/TransferDialog.tsx:20](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L20)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useTranslation } from "react-i18next";
import type { GroupDto } from "@/api/generated/tutoring-centre";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { SeatMeter } from "@/features/enrollments/SeatMeter";

/**
 * A radio list of groups with their teacher and seats, used to choose a transfer target or a group to enroll in.
 * Full groups, and groups the caller marks unavailable, are shown but disabled. The seat numbers are the server's
 * and only advisory: the request is still re-checked, and a 409 is shown by the caller.
 */
export function GroupChoiceList({
  groups,
  value,
  onChange,
  label,
  unavailableIds,
}: {
  groups: GroupDto[];
  value: string | null;
  onChange: (groupId: string) => void;
  label: string;
  unavailableIds?: ReadonlySet<string>;
}) {
  const { t } = useTranslation("enrollments");

  return (
    <RadioGroup
      aria-label={label}
      value={value}
      onValueChange={(next: string) => {
        onChange(next);
      }}
      className="max-h-80 overflow-y-auto"
    >
      {groups.map((group) => {
        const isFull = Number(group.enrolledCount) >= Number(group.capacity);
        const isUnavailable = unavailableIds?.has(group.id) ?? false;
        const disabled = isFull || isUnavailable;
        const labelId = `group-choice-${group.id}`;

        return (
          <label
            key={group.id}
            className={
              disabled
                ? "flex cursor-not-allowed items-center gap-3 rounded-lg border border-border p-3 text-muted-foreground"
                : "flex cursor-pointer items-center gap-3 rounded-lg border border-border p-3 hover:bg-muted/60 has-data-checked:border-primary has-data-checked:bg-primary/5"
            }
          >
            <RadioGroupItem value={group.id} disabled={disabled} aria-labelledby={labelId} />
            <span id={labelId} className="min-w-0 flex-1">
              <span className="block truncate font-medium">
                <bdi>{group.name}</bdi>
              </span>
              <span className="block truncate text-xs text-muted-foreground">
                <bdi>{group.teacherName}</bdi>
              </span>
              {isUnavailable && <span className="block text-xs">{t("groupPicker.alreadyEnrolled")}</span>}
            </span>
            <SeatMeter enrolled={Number(group.enrolledCount)} capacity={Number(group.capacity)} />
          </label>
        );
      })}
    </RadioGroup>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
