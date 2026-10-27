# `frontend/src/features/status/DemoBanner.tsx`

## Purpose

Source comment (intent): Non-dismissible, driven by the server's `demoMode` flag (Task 35.7) so it can never be forgotten in a build — it shows on every route, including the login page, and renders nothing until the flag is known to be true.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/status/DemoBanner.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 21 lines and 718 bytes. Source hash: `bc95dea3ae2ee903b65d7ed49fe764a45ff8f329f1e27dee2824c81857f5f766`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { useGetSystemInfo } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/features/status/DemoBanner.tsx#L8): `export function DemoBanner() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/status/DemoBanner.test.tsx:6](../../../../../../frontend/src/features/status/DemoBanner.test.tsx#L6)
- [frontend/src/routes/__root.tsx:3](../../../../../../frontend/src/routes/__root.tsx#L3)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useTranslation } from "react-i18next";
import { useGetSystemInfo } from "@/api/generated/tutoring-centre";

/**
 * Non-dismissible, driven by the server's `demoMode` flag (Task 35.7) so it can never be forgotten in a build —
 * it shows on every route, including the login page, and renders nothing until the flag is known to be true.
 */
export function DemoBanner() {
  const { t } = useTranslation("common");
  const { data, isPending } = useGetSystemInfo();

  if (isPending || data?.demoMode !== true) {
    return null;
  }

  return (
    <div className="w-full bg-amber-500 px-4 py-1.5 text-center text-sm font-medium text-amber-950" role="status">
      {t("demoBanner.message")}
    </div>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
