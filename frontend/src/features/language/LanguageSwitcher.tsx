import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";

/** Toggles between Arabic and English. The i18n module (@/i18n) persists the choice and flips document direction. */
export function LanguageSwitcher() {
  const { i18n, t } = useTranslation("common");
  const isArabic = i18n.language === "ar";

  const toggle = () => {
    void i18n.changeLanguage(isArabic ? "en" : "ar");
  };

  return (
    <Button type="button" variant="ghost" size="sm" onClick={toggle}>
      {isArabic ? t("language.english") : t("language.arabic")}
    </Button>
  );
}
