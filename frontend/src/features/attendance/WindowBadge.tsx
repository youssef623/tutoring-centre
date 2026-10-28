import { Clock, Lock, LockOpen } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Badge } from "@/components/ui/badge";
import { formatTime } from "@/features/schedule/centreTime";
import { isWindowState, windowOpensAt } from "./attendanceWindow";

/**
 * The attendance window as an icon, a word and a colour, taken from the server's `window` field and never
 * recomputed from the browser's clock. A window that has not opened says when it will, in the centre's zone.
 */
export function WindowBadge({
  window,
  startsAt,
  timeZoneId,
}: {
  window: string;
  startsAt: string;
  timeZoneId: string;
}) {
  const { t, i18n } = useTranslation("attendance");

  if (!isWindowState(window)) {
    return null;
  }

  if (window === "not_open") {
    const time = formatTime(windowOpensAt(startsAt), timeZoneId, i18n.language);

    return (
      <Badge variant="secondary">
        <Clock aria-hidden="true" />
        <span>{t("window.opensAt", { time })}</span>
      </Badge>
    );
  }

  if (window === "open") {
    return (
      <Badge>
        <LockOpen aria-hidden="true" />
        {t("window.open")}
      </Badge>
    );
  }

  return (
    <Badge variant="outline">
      <Lock aria-hidden="true" />
      {t("window.closed")}
    </Badge>
  );
}
