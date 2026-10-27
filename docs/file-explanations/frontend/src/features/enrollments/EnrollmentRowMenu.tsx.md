# `frontend/src/features/enrollments/EnrollmentRowMenu.tsx`

## Purpose

Source comment (intent): The per-enrollment actions, shared by the roster table and the student's enrollments card.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 1,395 bytes. Source hash: `9b01644feac3a745d997a8fdf59460459a20a775dc5f38193afed88d18b55c4c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { ArrowLeftRight, LogOut, MoreVertical } from "lucide-react";`
- Line 2: `import { useTranslation } from "react-i18next";`
- Line 3: `import { Button } from "@/components/ui/button";`
- Line 4: `import {`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L12): `export function EnrollmentRowMenu({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/GroupRosterTab.tsx:11](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L11)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:12](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L12)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { ArrowLeftRight, LogOut, MoreVertical } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

/** The per-enrollment actions, shared by the roster table and the student's enrollments card. */
export function EnrollmentRowMenu({
  label,
  onTransfer,
  onWithdraw,
}: {
  label: string;
  onTransfer: () => void;
  onWithdraw: () => void;
}) {
  const { t } = useTranslation("enrollments");

  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        render={<Button type="button" variant="ghost" size="icon-sm" aria-label={t("actions.openMenu", { name: label })} />}
      >
        <MoreVertical className="size-4" aria-hidden="true" />
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-auto">
        <DropdownMenuItem onClick={onTransfer}>
          <ArrowLeftRight className="size-4 rtl:-scale-x-100" aria-hidden="true" />
          {t("actions.transfer")}
        </DropdownMenuItem>
        <DropdownMenuItem variant="destructive" onClick={onWithdraw}>
          <LogOut className="size-4 rtl:-scale-x-100" aria-hidden="true" />
          {t("actions.withdraw")}
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
