import type { TFunction } from "i18next";

/** Fields whose before/after values are themselves translated vocabulary, not raw text. */
const StatusLikeFields = new Set(["status"]);

/**
 * Turns one changed field's raw before/after value into display text. Never returns a raw enum value or key:
 * - `role` reuses the "auth" namespace's role labels, so a role reads identically here and on the staff page.
 * - `defaultLocale` reuses the "common" namespace's language labels.
 * - `status` (subject active/archived, membership active/inactive) is translated within the "audit" namespace.
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

  if (StatusLikeFields.has(field)) {
    return t(`value.status.${value}`);
  }

  return value;
}
