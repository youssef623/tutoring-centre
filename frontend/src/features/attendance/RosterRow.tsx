import { History, Pencil, QrCode } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import type { AttendanceStatus, AttendanceStudentDto } from "@/api/generated/tutoring-centre";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { cn } from "cn";
import { isAttendanceStatus } from "./attendanceStatus";
import { isolateLtr } from "./bidi";
import { StatusToggle } from "./StatusToggle";

/**
 * One expected student. Their name leads (isolated, as names mix scripts), then what the record says about
 * how it was made — a QR scan, or a correction with how many times and by whom — then the toggle. The toggle
 * shows the draft when there is one, so a tap changes this row only; the record itself is never edited here.
 */
export function RosterRow({
  item,
  draftStatus,
  timeZoneId,
  markingDisabled,
  rowError,
  onChange,
  onCorrect,
}: {
  item: AttendanceStudentDto;
  draftStatus: AttendanceStatus | undefined;
  timeZoneId: string;
  markingDisabled: boolean;
  rowError: string | undefined;
  onChange: (status: AttendanceStatus) => void;
  onCorrect?: () => void;
}) {
  const { t, i18n } = useTranslation("attendance");
  const [detailOpen, setDetailOpen] = useState(false);
  const { record } = item;
  const recordStatus = isAttendanceStatus(record?.status) ? record.status : undefined;
  const shown = draftStatus ?? recordStatus;
  const isUnsaved = draftStatus !== undefined && draftStatus !== recordStatus;
  const correctionCount = record === null ? 0 : Number(record.correctionCount);

  const markedAt =
    record === null
      ? ""
      : isolateLtr(
          new Intl.DateTimeFormat(i18n.language, { dateStyle: "medium", timeStyle: "short", timeZone: timeZoneId }).format(
            new Date(record.markedAt),
          ),
        );

  return (
    <li className={cn("space-y-3 rounded-lg border border-border bg-card p-3", rowError !== undefined && "border-destructive")}>
      <div className="flex flex-wrap items-center justify-between gap-x-3 gap-y-2">
        <p dir="auto" className="min-w-0 break-words text-base font-semibold">
          <bdi>{item.studentName}</bdi>
        </p>

        <div className="flex flex-wrap items-center gap-1.5">
          {record === null && draftStatus === undefined && (
            <Badge variant="outline">{t("roster.notRecorded")}</Badge>
          )}
          {isUnsaved && <Badge variant="secondary">{t("roster.unsaved")}</Badge>}
          {record?.source === "qr" && (
            <Badge variant="secondary">
              <QrCode aria-hidden="true" />
              {t("source.qr")}
            </Badge>
          )}
          {correctionCount > 0 && (
            <Tooltip open={detailOpen} onOpenChange={setDetailOpen}>
              <TooltipTrigger
                render={
                  <button
                    type="button"
                    className="rounded-4xl outline-none focus-visible:ring-3 focus-visible:ring-ring/50"
                    onClick={() => {
                      setDetailOpen((open) => !open);
                    }}
                  />
                }
              >
                <Badge variant="outline">
                  <History aria-hidden="true" />
                  {t("roster.corrected", { count: correctionCount })}
                </Badge>
              </TooltipTrigger>
              <TooltipContent>
                {record?.markedByName
                  ? t("roster.correctedDetail", { name: record.markedByName, when: markedAt })
                  : t("roster.correctedDetailNoName", { when: markedAt })}
              </TooltipContent>
            </Tooltip>
          )}
        </div>
      </div>

      <StatusToggle studentName={item.studentName} value={shown} disabled={markingDisabled} onChange={onChange} />

      {rowError !== undefined && (
        <p role="alert" className="text-sm text-destructive">
          {rowError}
        </p>
      )}

      {record !== null && onCorrect !== undefined && (
        <Can permission={Permissions.AttendanceCorrect}>
          <Button type="button" variant="outline" size="sm" onClick={onCorrect}>
            <Pencil className="size-4" aria-hidden="true" />
            {t("roster.correct")}
            <span className="sr-only">
              {" "}
              <bdi>{item.studentName}</bdi>
            </span>
          </Button>
        </Can>
      )}
    </li>
  );
}
