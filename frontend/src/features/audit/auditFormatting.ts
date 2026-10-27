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
  "attendance_record",
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
