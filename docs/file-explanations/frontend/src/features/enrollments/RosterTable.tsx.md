# `frontend/src/features/enrollments/RosterTable.tsx`

## Purpose

Source comment (intent): The students currently in a group. Names link to the student page only for sessions that may open it (`students.view`); a group's teacher sees plain names. The row menu column exists only when the caller passes one, which the roster tab does only inside `enrollments.manage`. No contact details: the API returns none.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/RosterTable.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 59 lines and 2,303 bytes. Source hash: `7495fb7517e1260b427f4acb9e40f75c289ab5fa9fab399542d02b75e6ceec2b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link } from "@tanstack/react-router";`
- Line 2: `import type { ReactNode } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import type { RosterEntryDto } from "@/api/generated/tutoring-centre";`
- Line 5: `import {`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L19): `export function RosterTable({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/GroupRosterTab.tsx:13](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link } from "@tanstack/react-router";
import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import type { RosterEntryDto } from "@/api/generated/tutoring-centre";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";

/**
 * The students currently in a group. Names link to the student page only for sessions that may open it
 * (`students.view`); a group's teacher sees plain names. The row menu column exists only when the caller passes
 * one, which the roster tab does only inside `enrollments.manage`. No contact details: the API returns none.
 */
export function RosterTable({
  items,
  linkStudents,
  rowMenu,
}: {
  items: RosterEntryDto[];
  linkStudents: boolean;
  rowMenu?: (entry: RosterEntryDto) => ReactNode;
}) {
  const { t, i18n } = useTranslation("enrollments");
  const dateFormat = new Intl.DateTimeFormat(i18n.language, { dateStyle: "medium" });

  return (
    <DataTable className="sm:min-w-[480px]">
      <DataTableHead>
        <DataTableColumn>{t("roster.columns.student")}</DataTableColumn>
        <DataTableColumn>{t("roster.columns.enrolledAt")}</DataTableColumn>
        {rowMenu !== undefined && <DataTableColumn hidden>{t("roster.columns.actions")}</DataTableColumn>}
      </DataTableHead>
      <DataTableBody>
        {items.map((entry) => (
          <DataTableRow key={entry.enrollmentId}>
            <DataTableCell kind="primary">
              {linkStudents ? (
                <Link to="/students/$studentId" params={{ studentId: entry.studentId }} className="font-medium hover:underline">
                  <bdi>{entry.studentName}</bdi>
                </Link>
              ) : (
                <bdi className="font-medium">{entry.studentName}</bdi>
              )}
            </DataTableCell>
            <DataTableCell label={t("roster.columns.enrolledAt")} className="text-muted-foreground">
              <time dateTime={entry.enrolledAt}>{dateFormat.format(new Date(entry.enrolledAt))}</time>
            </DataTableCell>
            {rowMenu !== undefined && <DataTableCell kind="actions">{rowMenu(entry)}</DataTableCell>}
          </DataTableRow>
        ))}
      </DataTableBody>
    </DataTable>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
