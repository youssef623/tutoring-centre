import i18next from "i18next";
import ICU from "i18next-icu";
import { initReactI18next } from "react-i18next";
import commonAr from "./locales/ar/common.json";
import errorsAr from "./locales/ar/errors.json";
import statusAr from "./locales/ar/status.json";
import commonEn from "./locales/en/common.json";
import errorsEn from "./locales/en/errors.json";
import statusEn from "./locales/en/status.json";

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
    lng: "en",
    fallbackLng: "en",
    ns: ["common", "status", "errors"],
    defaultNS: "common",
    interpolation: { escapeValue: false },
  });

export default i18next;
