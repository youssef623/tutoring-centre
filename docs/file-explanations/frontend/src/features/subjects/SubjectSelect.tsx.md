# `frontend/src/features/subjects/SubjectSelect.tsx`

## Purpose

Source comment (intent): A select backed by the subjects list, offering active subjects only (Day 46). Never free text: the value is a subject ID, and the server still validates it (unknown, foreign or archived → a refusal shown by the caller under this select). With `allLabel`, a first "all" entry maps to null for use as a list filter.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/subjects/SubjectSelect.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 77 lines and 2,951 bytes. Source hash: `933b384d101c0c2d8207748e34f4ecc2a8509395982e0e85d4dde828d832624a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link } from "@tanstack/react-router";`
- Line 2: `import { useTranslation } from "react-i18next";`
- Line 3: `import { useListSubjects } from "@/api/generated/tutoring-centre";`
- Line 4: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../frontend/src/features/subjects/SubjectSelect.tsx#L13): `export function SubjectSelect({`
- [Line 48](../../../../../../frontend/src/features/subjects/SubjectSelect.tsx#L48): `// The render function replaces Base UI's placeholder entirely, so "nothing chosen" renders the hint itself.`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:23](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L23)
- [frontend/src/features/groups/CreateGroupDialog.tsx:30](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L30)
- [frontend/src/routes/_authenticated/groups.index.tsx:13](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { useListSubjects } from "@/api/generated/tutoring-centre";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const AllValue = "__all";

/**
 * A select backed by the subjects list, offering active subjects only (Day 46). Never free text: the value is
 * a subject ID, and the server still validates it (unknown, foreign or archived → a refusal shown by the
 * caller under this select). With `allLabel`, a first "all" entry maps to null for use as a list filter.
 */
export function SubjectSelect({
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
  const { data, isPending, isError } = useListSubjects({ includeArchived: false });
  const subjects = data?.items ?? [];
  const nameOf = (subjectId: string) => subjects.find((subject) => subject.id === subjectId)?.name ?? "";

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
              <span className="text-muted-foreground">{t(isPending ? "selects.loading" : "selects.subjectPlaceholder")}</span>
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
        {subjects.map((subject) => (
          <SelectItem key={subject.id} value={subject.id}>
            <span dir="auto">{subject.name}</span>
          </SelectItem>
        ))}
        {!isPending && subjects.length === 0 && (
          <div className="space-y-1 px-2 py-2 text-sm">
            <p className="text-muted-foreground">{t(isError ? "selects.loadFailed" : "selects.noSubjects")}</p>
            <Link to="/subjects" search={{ archived: false }} className="font-medium text-primary underline-offset-2 hover:underline">
              {t("selects.goToSubjects")}
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
