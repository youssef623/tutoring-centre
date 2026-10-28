import { History, QrCode } from "lucide-react";
import { useTranslation } from "react-i18next";
import type { AttendanceHistoryItemDto } from "@/api/generated/tutoring-centre";
import { isAttendanceStatus } from "./attendanceStatus";

const colors = {
  present: "bg-emerald-100 text-emerald-900 dark:bg-emerald-950 dark:text-emerald-200",
  late: "bg-amber-100 text-amber-900 dark:bg-amber-950 dark:text-amber-200",
  absent: "bg-red-100 text-red-900 dark:bg-red-950 dark:text-red-200",
  excused: "bg-blue-100 text-blue-900 dark:bg-blue-950 dark:text-blue-200",
};

export function HistoryStatus({ item }: { item: AttendanceHistoryItemDto }) {
  const { t } = useTranslation("attendance");
  const status = isAttendanceStatus(item.status) ? item.status : undefined;
  return (
    <span className="inline-flex items-center gap-1.5">
      <span
        className={`inline-flex size-8 items-center justify-center rounded-md font-semibold ${status === undefined ? "" : colors[status]}`}
        role="img"
        aria-label={status === undefined ? item.status : t(`status.${status}`)}
      >
        {status === undefined ? "?" : t(`history.letters.${status}`)}
      </span>
      {item.source === "qr" && (
        <span role="img" aria-label={t("source.qr")}>
          <QrCode className="size-4" aria-hidden="true" />
        </span>
      )}
      {Number(item.correctionCount) > 0 && (
        <span
          role="img"
          aria-label={t("roster.corrected", { count: Number(item.correctionCount) })}
        >
          <History className="size-4" aria-hidden="true" />
        </span>
      )}
    </span>
  );
}
