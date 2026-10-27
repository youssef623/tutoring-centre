# `frontend/src/features/audit/auditFormatting.ts`

## Purpose

Source comment (intent): Fields whose before/after values are themselves translated vocabulary, not raw text — looked up as "value.<field>.<value>" in the "audit" namespace.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/audit/auditFormatting.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 59 lines and 2,075 bytes. Source hash: `7b135e56a36933235978ab86f1aa8f6a6413dca8a84f3c63defd386be3ef24b5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { TFunction } from "i18next";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/features/audit/auditFormatting.ts#L8): `export const AuditEntityTypes = [`
- [Line 22](../../../../../../frontend/src/features/audit/auditFormatting.ts#L22): `export type AuditEntityType = (typeof AuditEntityTypes)[number];`
- [Line 25](../../../../../../frontend/src/features/audit/auditFormatting.ts#L25): `* Turns one changed field's raw before/after value into display text. Never returns a raw enum value or key:`
- [Line 35](../../../../../../frontend/src/features/audit/auditFormatting.ts#L35): `export function formatAuditValue(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/assemble.py:9](../../../../../onboarding-work/assemble.py#L9)
- [docs/onboarding-work/extend_backend_purposes.py:12](../../../../../onboarding-work/extend_backend_purposes.py#L12)
- [frontend/e2e/full-centre-setup.spec.ts:26](../../../../../../frontend/e2e/full-centre-setup.spec.ts#L26)
- [frontend/e2e/schedule.spec.ts:18](../../../../../../frontend/e2e/schedule.spec.ts#L18)
- [frontend/scripts/check-i18n-keys.mjs:13](../../../../../../frontend/scripts/check-i18n-keys.mjs#L13)
- [frontend/src/api/generated/tutoring-centre.ts:981](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L981)
- [frontend/src/api/problemDetails.test.ts:7](../../../../../../frontend/src/api/problemDetails.test.ts#L7)
- [frontend/src/api/problemDetails.ts:16](../../../../../../frontend/src/api/problemDetails.ts#L16)
- [frontend/src/components/shared/ParentLookupField.tsx:16](../../../../../../frontend/src/components/shared/ParentLookupField.tsx#L16)
- [frontend/src/components/shared/PhoneInput.test.tsx:9](../../../../../../frontend/src/components/shared/PhoneInput.test.tsx#L9)
- [frontend/src/components/shared/PhoneInput.tsx:11](../../../../../../frontend/src/components/shared/PhoneInput.tsx#L11)
- [frontend/src/components/shared/PhoneText.test.tsx:9](../../../../../../frontend/src/components/shared/PhoneText.test.tsx#L9)
- [frontend/src/components/shared/PhoneText.tsx:8](../../../../../../frontend/src/components/shared/PhoneText.tsx#L8)
- [frontend/src/components/shared/SearchInput.test.tsx:16](../../../../../../frontend/src/components/shared/SearchInput.test.tsx#L16)
- [frontend/src/components/shared/SearchInput.tsx:15](../../../../../../frontend/src/components/shared/SearchInput.tsx#L15)
- [frontend/src/components/shared/StatTile.tsx:7](../../../../../../frontend/src/components/shared/StatTile.tsx#L7)
- [frontend/src/components/ui/select.tsx:21](../../../../../../frontend/src/components/ui/select.tsx#L21)
- [frontend/src/features/audit/AuditLog.tsx:12](../../../../../../frontend/src/features/audit/AuditLog.tsx#L12)
- [frontend/src/features/dashboard/DashboardOverview.tsx:107](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L107)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:107](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L107)
- 122 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import type { TFunction } from "i18next";

/** Fields whose before/after values are themselves translated vocabulary, not raw text — looked up as
 * "value.<field>.<value>" in the "audit" namespace. */
const TranslatedValueFields = new Set(["status", "relationship", "isPrimary", "endReason", "dayOfWeek"]);

/** Every entity type the audit log records (docs/architecture/audit.md), in the order the filter lists them. */
export const AuditEntityTypes = [
  "subject",
  "membership",
  "centre",
  "student",
  "parent",
  "student_parent",
  "teacher",
  "group",
  "enrollment",
  "schedule_rule",
  "class_session",
] as const;

export type AuditEntityType = (typeof AuditEntityTypes)[number];

/**
 * Turns one changed field's raw before/after value into display text. Never returns a raw enum value or key:
 * - `role` reuses the "auth" namespace's role labels, so a role reads identically here and on the staff page.
 * - `defaultLocale` reuses the "common" namespace's language labels.
 * - `status` (subject/student active/archived, membership active/inactive, enrollment active/ended),
 *   `relationship` (father/mother/guardian/other), `isPrimary` (true/false) and `endReason`
 *   (withdrawn/transferred) are translated within the "audit" namespace.
 * - `phone` is already masked server-side (Task 38.6) — rendered as plain text, like any other string.
 * - anything else (name, userId) is rendered as plain text — a user id has no human-readable translation.
 * - `null` becomes the namespace's null-value dash, never a blank cell.
 */
export function formatAuditValue(
  field: string,
  value: string | null,
  t: TFunction<"audit">,
  tAuth: TFunction<"auth">,
  tCommon: TFunction<"common">,
): string {
  if (value === null) {
    return t("nullValue");
  }

  if (field === "role") {
    return tAuth(`roles.${value}`);
  }

  if (field === "defaultLocale") {
    return tCommon(value === "ar" ? "language.arabic" : "language.english");
  }

  if (TranslatedValueFields.has(field)) {
    return t(`value.${field}.${value}`);
  }

  return value;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
