import type { ApiError, ErrorKind } from "@/api/errors";

type Lang = "en" | "ar";
type Messages = Record<Lang, string>;

// Specific, known error codes. Extend as new codes appear; Day 17 moves these to i18n files.
const codeMessages: Record<string, Messages> = {
  "validation.failed": {
    en: "One or more fields are invalid.",
    ar: "حقل واحد أو أكثر غير صالح.",
  },
  "centre.name_required": {
    en: "Centre name is required.",
    ar: "اسم المركز مطلوب.",
  },
  "centre.name_too_long": {
    en: "Centre name is too long.",
    ar: "اسم المركز طويل جدًا.",
  },
  "centre.slug_invalid": {
    en: "Centre slug is invalid.",
    ar: "معرف المركز غير صالح.",
  },
  "centre.time_zone_invalid": {
    en: "Time zone is invalid.",
    ar: "المنطقة الزمنية غير صالحة.",
  },
};

// Fallback per error kind, when the specific code isn't in the dictionary above.
// Partial (not Record<ErrorKind, Messages>): an error.kind value outside the known union
// (an API response ESLint's static types can't see) must still fall through to the generic message.
const kindMessages: Partial<Record<ErrorKind, Messages>> = {
  validation: {
    en: "One or more fields are invalid.",
    ar: "حقل واحد أو أكثر غير صالح.",
  },
  notFound: {
    en: "The requested item could not be found.",
    ar: "لم يتم العثور على العنصر المطلوب.",
  },
  conflict: {
    en: "This action conflicts with existing data.",
    ar: "يتعارض هذا الإجراء مع بيانات موجودة.",
  },
  rule: {
    en: "This action isn't allowed right now.",
    ar: "هذا الإجراء غير مسموح به حاليًا.",
  },
  forbidden: {
    en: "You don't have permission to do this.",
    ar: "ليس لديك إذن للقيام بذلك.",
  },
  unexpected: {
    en: "Something went wrong.",
    ar: "حدث خطأ ما.",
  },
};

const genericMessage: Messages = {
  en: "Something went wrong. Please try again.",
  ar: "حدث خطأ ما. حاول مرة أخرى.",
};

export function messageFor(error: ApiError, lang: Lang): string {
  return (
    codeMessages[error.code]?.[lang] ?? kindMessages[error.kind]?.[lang] ?? genericMessage[lang]
  );
}
