import cardsEn from "./locales/en/cards.json";
import cardsAr from "./locales/ar/cards.json";
import scannerEn from "./locales/en/scanner.json";
import scannerAr from "./locales/ar/scanner.json";
import i18next from "i18next";
import ICU from "i18next-icu";
import { initReactI18next } from "react-i18next";
import attendanceAr from "./locales/ar/attendance.json";
import auditAr from "./locales/ar/audit.json";
import authAr from "./locales/ar/auth.json";
import commonAr from "./locales/ar/common.json";
import enrollmentsAr from "./locales/ar/enrollments.json";
import errorsAr from "./locales/ar/errors.json";
import groupsAr from "./locales/ar/groups.json";
import parentsAr from "./locales/ar/parents.json";
import scheduleAr from "./locales/ar/schedule.json";
import settingsAr from "./locales/ar/settings.json";
import shellAr from "./locales/ar/shell.json";
import staffAr from "./locales/ar/staff.json";
import statusAr from "./locales/ar/status.json";
import studentsAr from "./locales/ar/students.json";
import subjectsAr from "./locales/ar/subjects.json";
import teachersAr from "./locales/ar/teachers.json";
import attendanceEn from "./locales/en/attendance.json";
import auditEn from "./locales/en/audit.json";
import authEn from "./locales/en/auth.json";
import commonEn from "./locales/en/common.json";
import enrollmentsEn from "./locales/en/enrollments.json";
import errorsEn from "./locales/en/errors.json";
import groupsEn from "./locales/en/groups.json";
import parentsEn from "./locales/en/parents.json";
import scheduleEn from "./locales/en/schedule.json";
import settingsEn from "./locales/en/settings.json";
import shellEn from "./locales/en/shell.json";
import staffEn from "./locales/en/staff.json";
import statusEn from "./locales/en/status.json";
import studentsEn from "./locales/en/students.json";
import subjectsEn from "./locales/en/subjects.json";
import teachersEn from "./locales/en/teachers.json";

const StorageKey = "tcm.lang";
type SupportedLanguage = "ar" | "en";

function isSupportedLanguage(value: unknown): value is SupportedLanguage {
  return value === "ar" || value === "en";
}

/** The persisted choice, else the browser language, else English. Only the language preference ever goes into localStorage. */
function detectInitialLanguage(): SupportedLanguage {
  try {
    const stored = localStorage.getItem(StorageKey);
    if (isSupportedLanguage(stored)) {
      return stored;
    }
  } catch {
    // localStorage can throw (private browsing, blocked storage); fall through to browser detection.
  }

  const browserLanguage = typeof navigator === "undefined" ? "en" : navigator.language;
  return browserLanguage.toLowerCase().startsWith("ar") ? "ar" : "en";
}

/** Keeps <html> in sync with the current language: direction is a document-level property driven by language. */
function applyDocumentDirection(language: string): void {
  if (typeof document === "undefined") {
    return;
  }

  document.documentElement.lang = language;
  document.documentElement.dir = language === "ar" ? "rtl" : "ltr";
}

const initialLanguage = detectInitialLanguage();
applyDocumentDirection(initialLanguage);

/**
 * Resources are bundled at build time (no runtime fetch for two small languages). ICU message format
 * (single-brace placeholders, e.g. "{time}") replaces i18next's default interpolation so plural rules —
 * Arabic has six — can be expressed later without a different templating syntax.
 */
void i18next
  .use(ICU)
  .use(initReactI18next)
  .init({
    resources: {
      en: {
        common: commonEn,
        status: statusEn,
        errors: errorsEn,
        auth: authEn,
        shell: shellEn,
        subjects: subjectsEn,
        students: studentsEn,
        parents: parentsEn,
        staff: staffEn,
        settings: settingsEn,
        audit: auditEn,
        teachers: teachersEn,
        groups: groupsEn,
        enrollments: enrollmentsEn,
        schedule: scheduleEn,
        attendance: attendanceEn,
        cards: cardsEn,
        scanner: scannerEn,
      },
      ar: {
        common: commonAr,
        status: statusAr,
        errors: errorsAr,
        auth: authAr,
        shell: shellAr,
        subjects: subjectsAr,
        students: studentsAr,
        parents: parentsAr,
        staff: staffAr,
        settings: settingsAr,
        audit: auditAr,
        teachers: teachersAr,
        groups: groupsAr,
        enrollments: enrollmentsAr,
        schedule: scheduleAr,
        attendance: attendanceAr,
        cards: cardsAr,
        scanner: scannerAr,
      },
    },
    lng: initialLanguage,
    fallbackLng: "en",
    ns: ["common", "status", "errors", "auth", "shell", "subjects", "students", "parents", "staff", "settings", "audit", "teachers", "groups", "enrollments", "schedule", "attendance", "cards", "scanner"],
    defaultNS: "common",
    interpolation: { escapeValue: false },
  });

i18next.on("languageChanged", (language) => {
  applyDocumentDirection(language);
  try {
    localStorage.setItem(StorageKey, language);
  } catch {
    // Best-effort persistence only.
  }
});

export default i18next;
