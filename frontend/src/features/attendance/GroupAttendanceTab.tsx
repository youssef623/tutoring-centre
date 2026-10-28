import { Link } from "@tanstack/react-router";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useGetAttendanceInfinite } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { formatTime } from "@/features/schedule/centreTime";
import { addDays, formatCalendarDate } from "@/features/schedule/week";
import { HistoryState } from "./HistoryState";
import { HistoryStatus } from "./HistoryStatus";
import { useAttendanceRange } from "./useAttendanceRange";

export function GroupAttendanceTab({
  groupId,
  from,
  to,
  onRangeChange,
}: {
  groupId: string;
  from?: string;
  to?: string;
  onRangeChange: (range: { from: string; to: string }) => void;
}) {
  const { t, i18n } = useTranslation("attendance");
  const range = useAttendanceRange(from, to);
  const query = useGetAttendanceInfinite(
    { groupId, from: range.from, to: range.to },
    {
      query: {
        enabled: range.valid,
        initialPageParam: undefined,
        getNextPageParam: (page) => page.nextCursor ?? undefined,
      },
    },
  );

  const { hasNextPage, isFetching, isError, fetchNextPage } = query;

  // The grid is complete only once the last page lands. A failed page waits for Retry.
  useEffect(() => {
    if (range.valid && hasNextPage && !isFetching && !isError) void fetchNextPage();
  }, [range.valid, hasNextPage, isFetching, isError, fetchNextPage]);

  useEffect(() => {
    if (
      (from === undefined || to === undefined) &&
      range.from !== undefined &&
      range.to !== undefined
    ) {
      onRangeChange({ from: range.from, to: range.to });
    }
  }, [from, to, range.from, range.to, onRangeChange]);

  const records = query.data?.pages.flatMap((page) => page.items) ?? [];
  const sessions = [...new Map(records.map((item) => [item.sessionId, item])).values()].sort(
    (a, b) => a.startsAt.localeCompare(b.startsAt),
  );
  const students = [...new Map(records.map((item) => [item.studentId, item])).values()].sort(
    (a, b) => a.studentName.localeCompare(b.studentName, i18n.language),
  );
  const cells = new Map(records.map((item) => [`${item.studentId}/${item.sessionId}`, item]));

  return (
    <section className="min-w-0 space-y-4">
      <div className="flex flex-wrap items-end gap-3">
        <label className="space-y-1 text-sm">
          {t("history.from")}
          <Input
            type="date"
            dir="ltr"
            value={range.from ?? ""}
            onChange={(event) => {
              onRangeChange({ from: event.target.value, to: range.to ?? "" });
            }}
          />
        </label>
        <label className="space-y-1 text-sm">
          {t("history.to")}
          <Input
            type="date"
            dir="ltr"
            value={range.to ?? ""}
            onChange={(event) => {
              onRangeChange({ from: range.from ?? "", to: event.target.value });
            }}
          />
        </label>
        <Button
          variant="outline"
          disabled={range.today === undefined}
          onClick={() => {
            if (range.today !== undefined)
              onRangeChange({ from: addDays(range.today, -27), to: range.today });
          }}
        >
          {t("history.lastFourWeeks")}
        </Button>
      </div>
      {range.timeZoneId !== null && !range.valid ? (
        <p role="alert">{t("history.invalidRange")}</p>
      ) : query.isPending ||
        query.isFetching ||
        query.hasNextPage ||
        query.isError ||
        records.length === 0 ? (
        <HistoryState
          loading={query.isPending || query.isFetching || query.hasNextPage}
          error={query.error}
          onRetry={() => {
            void query.refetch();
          }}
        />
      ) : (
        <div
          className="max-w-full overflow-x-auto rounded-lg border"
          role="region"
          aria-label={t("history.title")}
          tabIndex={0}
        >
          <table className="w-full text-sm">
            <caption className="sr-only">{t("history.title")}</caption>
            <thead>
              <tr>
                <th scope="col" className="sticky start-0 z-10 bg-card p-3 text-start">
                  {t("history.student")}
                </th>
                {sessions.map((session) => (
                  <th scope="col" key={session.sessionId} className="whitespace-nowrap p-3">
                    <Link
                      to="/attendance/sessions/$sessionId"
                      params={{ sessionId: session.sessionId }}
                      className="underline underline-offset-4"
                    >
                      <span dir="ltr">
                        {formatCalendarDate(session.localDate, i18n.language)}
                        <br />
                        {formatTime(session.startsAt, range.timeZoneId ?? "UTC", i18n.language)}
                      </span>
                    </Link>
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.studentId} className="border-t">
                  <th
                    scope="row"
                    className="sticky start-0 z-10 whitespace-nowrap bg-card p-3 text-start"
                  >
                    <bdi>{student.studentName}</bdi>
                  </th>
                  {sessions.map((session) => {
                    const item = cells.get(`${student.studentId}/${session.sessionId}`);
                    return (
                      <td key={session.sessionId} className="p-3 text-center">
                        {item === undefined ? (
                          <span role="img" aria-label={t("roster.notRecorded")}>
                            —
                          </span>
                        ) : (
                          <HistoryStatus item={item} />
                        )}
                      </td>
                    );
                  })}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
