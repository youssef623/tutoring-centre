import type { ApiError } from "./errors";

export type Lang = "en" | "ar";

type Dictionary = Partial<Record<string, string>>;

// The backend owns stable codes; the frontend owns wording and language. Arabic strings are placeholders
// until Day 17 moves all text to i18n files.
const byCode: Record<Lang, Dictionary> = {
  en: {
    "validation.failed": "Some fields are invalid. Please check the form.",
    "centre.name_required": "Centre name is required.",
    "centre.name_too_long": "Centre name must be at most 120 characters.",
    "centre.slug_invalid":
      "The web address may only contain lowercase letters, digits and single hyphens.",
    "centre.time_zone_invalid": "The time zone is not recognised.",
    "centre.slug_taken": "A centre with this web address already exists.",
    "centre.create_forbidden": "Only the platform can create centres.",
  },
  ar: {
    "validation.failed": "بعض الحقول غير صالحة. يرجى مراجعة النموذج.",
    "centre.name_required": "اسم المركز مطلوب.",
    "centre.name_too_long": "يجب ألا يزيد اسم المركز عن 120 حرفًا.",
    "centre.slug_invalid": "يجب أن يحتوي العنوان على أحرف إنجليزية صغيرة وأرقام وشرطات مفردة فقط.",
    "centre.time_zone_invalid": "المنطقة الزمنية غير معروفة.",
    "centre.slug_taken": "يوجد مركز بهذا العنوان بالفعل.",
    "centre.create_forbidden": "إنشاء المراكز متاح للمنصة فقط.",
  },
};

const byKind: Record<Lang, Dictionary> = {
  en: {
    validation: "Please check the highlighted fields.",
    notFound: "We could not find what you asked for.",
    conflict: "This conflicts with existing data.",
    rule: "This action is not allowed right now.",
    forbidden: "You do not have permission to do this.",
    unexpected: "Something went wrong. Please try again.",
  },
  ar: {
    validation: "يرجى مراجعة الحقول المحددة.",
    notFound: "لم نتمكن من العثور على ما طلبته.",
    conflict: "يتعارض هذا مع بيانات موجودة.",
    rule: "هذا الإجراء غير مسموح به حاليًا.",
    forbidden: "ليس لديك صلاحية لتنفيذ هذا الإجراء.",
    unexpected: "حدث خطأ ما. يرجى المحاولة مرة أخرى.",
  },
};

const generic: Record<Lang, string> = {
  en: "Something went wrong. Please try again.",
  ar: "حدث خطأ ما. يرجى المحاولة مرة أخرى.",
};

/** The one function every form and toast uses to turn an ApiError into user-facing text. */
export function messageFor(error: ApiError, lang: Lang): string {
  return byCode[lang][error.code] ?? byKind[lang][error.kind] ?? generic[lang];
}
