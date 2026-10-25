import { useTranslation } from "react-i18next";

/** A length in minutes as people say it: "45 minutes", "1 hour 30 minutes", "2 hours". */
export function useDurationLabel() {
  const { t } = useTranslation("schedule");

  return (minutes: number) =>
    minutes < 60
      ? t("durations.minutes", { minutes })
      : t("durations.hoursAndMinutes", { hours: Math.floor(minutes / 60), minutes: minutes % 60 });
}
