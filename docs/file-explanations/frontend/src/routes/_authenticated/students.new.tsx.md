# `frontend/src/routes/_authenticated/students.new.tsx`

## Purpose

Declares `RegisterStudentPage`, `RegisterStudentPageContent`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/students.new.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 855 bytes. Source hash: `6fb3f854c2a1de983081d43baafdd5e0cd2a422856cba2b1b023b07f7604ac60`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute } from "@tanstack/react-router";`
- Line 2: `import { useTranslation } from "react-i18next";`
- Line 3: `import { RegisterStudentForm } from "@/features/students/RegisterStudentForm";`
- Line 4: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 5: `import { Permissions } from "@/features/session/permissions";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/routes/_authenticated/students.new.tsx#L7): `export const Route = createFileRoute("/_authenticated/students/new")({`
- [Line 11](../../../../../../frontend/src/routes/_authenticated/students.new.tsx#L11): `function RegisterStudentPage() {`
- [Line 19](../../../../../../frontend/src/routes/_authenticated/students.new.tsx#L19): `function RegisterStudentPageContent() {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { RegisterStudentForm } from "@/features/students/RegisterStudentForm";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

export const Route = createFileRoute("/_authenticated/students/new")({
  component: RegisterStudentPage,
});

function RegisterStudentPage() {
  return (
    <RouteGuard permission={Permissions.StudentsManage}>
      <RegisterStudentPageContent />
    </RouteGuard>
  );
}

function RegisterStudentPageContent() {
  const { t } = useTranslation("students");

  return (
    <section className="space-y-4">
      <h1 className="text-2xl font-semibold tracking-tight">{t("actions.register")}</h1>
      <RegisterStudentForm />
    </section>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
