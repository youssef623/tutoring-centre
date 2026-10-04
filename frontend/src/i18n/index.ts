import i18next from "i18next";
import ICU from "i18next-icu";
import { initReactI18next } from "react-i18next";
import commonAr from "./locales/ar/common.json";
import errorsAr from "./locales/ar/errors.json";
import statusAr from "./locales/ar/status.json";
import commonEn from "./locales/en/common.json";
import errorsEn from "./locales/en/errors.json";
import statusEn from "./locales/en/status.json";

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
      en: { common: commonEn, status: statusEn, errors: errorsEn },
      ar: { common: commonAr, status: statusAr, errors: errorsAr },
    },
    lng: initialLanguage,
    fallbackLng: "en",
    ns: ["common", "status", "errors"],
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
