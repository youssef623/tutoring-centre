# `frontend/src/features/teachers/TeacherSelect.tsx`

## Purpose

Source comment (intent): A select backed by the teachers list, offering active teachers only (Day 46). Calls `GET /api/teachers`, which needs `teachers.view`: a caller must render this only for a session holding it (behind `Can`), never rely on a 403 to hide it. The server still validates the chosen ID. With `allLabel`, a first "all" entry maps to null for use as a list filter.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/teachers/TeacherSelect.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 78 lines and 2,973 bytes. Source hash: `11fb5094bcf8b84b2a2dddd4cceacbc55a596e3b01bde91365da87d4ffabba07`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link } from "@tanstack/react-router";`
- Line 2: `import { useTranslation } from "react-i18next";`
- Line 3: `import { useListTeachers } from "@/api/generated/tutoring-centre";`
- Line 4: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../frontend/src/features/teachers/TeacherSelect.tsx#L14): `export function TeacherSelect({`
- [Line 49](../../../../../../frontend/src/features/teachers/TeacherSelect.tsx#L49): `// The render function replaces Base UI's placeholder entirely, so "nothing chosen" renders the hint itself.`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/groups/CreateGroupDialog.tsx:31](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L31)
- [frontend/src/features/groups/GroupEditDialogs.tsx:41](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L41)
- [frontend/src/features/schedule/TimetablePage.tsx:25](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L25)
- [frontend/src/routes/_authenticated/groups.index.tsx:14](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { useListTeachers } from "@/api/generated/tutoring-centre";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const AllValue = "__all";

/**
 * A select backed by the teachers list, offering active teachers only (Day 46). Calls `GET /api/teachers`,
 * which needs `teachers.view`: a caller must render this only for a session holding it (behind `Can`), never
 * rely on a 403 to hide it. The server still validates the chosen ID. With `allLabel`, a first "all" entry
 * maps to null for use as a list filter.
 */
export function TeacherSelect({
  id,
  value,
  onChange,
  allLabel,
  invalid = false,
  describedBy,
  disabled = false,
  className = "w-full",
}: {
  id: string;
  value: string | null;
  onChange: (value: string | null) => void;
  allLabel?: string;
  invalid?: boolean;
  describedBy?: string;
  disabled?: boolean;
  className?: string;
}) {
  const { t } = useTranslation("groups");
  const { data, isPending, isError } = useListTeachers({ includeArchived: false });
  const teachers = data?.items ?? [];
  const nameOf = (teacherId: string) => teachers.find((teacher) => teacher.id === teacherId)?.fullName ?? "";

  return (
    <Select
      value={value ?? (allLabel === undefined ? null : AllValue)}
      onValueChange={(next: string | null) => {
        onChange(next === null || next === AllValue ? null : next);
      }}
      disabled={disabled || isPending}
    >
      <SelectTrigger id={id} className={className} aria-invalid={invalid} aria-describedby={describedBy}>
        <SelectValue>
          {(selected: string | null) =>
            // The render function replaces Base UI's placeholder entirely, so "nothing chosen" renders the hint itself.
            selected === null ? (
              <span className="text-muted-foreground">{t(isPending ? "selects.loading" : "selects.teacherPlaceholder")}</span>
            ) : selected === AllValue ? (
              allLabel
            ) : (
              <span dir="auto">{nameOf(selected)}</span>
            )
          }
        </SelectValue>
      </SelectTrigger>
      <SelectContent>
        {allLabel !== undefined && <SelectItem value={AllValue}>{allLabel}</SelectItem>}
        {teachers.map((teacher) => (
          <SelectItem key={teacher.id} value={teacher.id}>
            <span dir="auto">{teacher.fullName}</span>
          </SelectItem>
        ))}
        {!isPending && teachers.length === 0 && (
          <div className="space-y-1 px-2 py-2 text-sm">
            <p className="text-muted-foreground">{t(isError ? "selects.loadFailed" : "selects.noTeachers")}</p>
            <Link to="/teachers" className="font-medium text-primary underline-offset-2 hover:underline">
              {t("selects.goToTeachers")}
            </Link>
          </div>
        )}
      </SelectContent>
    </Select>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
