import { useTranslation } from "react-i18next";
import { useGetAttendanceInfinite } from "@/api/generated/tutoring-centre";
import { LoadMoreButton } from "@/components/shared/LoadMoreButton";
import { formatTime } from "@/features/schedule/centreTime";
import { formatCalendarDate } from "@/features/schedule/week";
import { AttendanceStatusOrder } from "./attendanceStatus";
import { HistoryState } from "./HistoryState";
import { HistoryStatus } from "./HistoryStatus";
import { useAttendanceRange } from "./useAttendanceRange";

export function StudentAttendanceCard({ studentId }: { studentId: string }) {
  const { t, i18n } = useTranslation("attendance");
  const range = useAttendanceRange(undefined, undefined, 30);
  const query = useGetAttendanceInfinite(
    { studentId, from: range.from, to: range.to },
    {
      query: {
        enabled: range.valid,
        initialPageParam: undefined,
        getNextPageParam: (page) => page.nextCursor ?? undefined,
      },
    },
  );
  const records = query.data?.pages.flatMap((page) => page.items) ?? [];
  return (
    <section className="space-y-4 rounded-xl border border-border bg-card p-4">
      <h2 className="text-lg font-semibold">{t("history.title")}</h2>
      {range.from !== undefined && range.to !== undefined && (
        <p className="text-sm text-muted-foreground" dir="ltr">
          {formatCalendarDate(range.from, i18n.language)} –{" "}
          {formatCalendarDate(range.to, i18n.language)}
        </p>
      )}
      {records.length > 0 && (
        <p>
          {t(
            "history.counts",
            Object.fromEntries(
              AttendanceStatusOrder.map((status) => [
                status,
                records.filter((item) => item.status === status).length,
              ]),
            ),
          )}
        </p>
      )}
      {query.isPending || query.isError || records.length === 0 ? (
        <HistoryState
          loading={query.isPending}
          error={query.error}
          onRetry={() => {
            void query.refetch();
          }}
        />
      ) : (
        <ul className="divide-y">
          {records.map((item) => (
            <li
              key={item.recordId}
              className="flex flex-wrap items-center justify-between gap-3 py-3"
            >
              <div>
                <p dir="ltr">
                  {formatCalendarDate(item.localDate, i18n.language)} ·{" "}
                  {formatTime(item.startsAt, range.timeZoneId ?? "UTC", i18n.language)}
                </p>
                <p>
                  <bdi>{item.groupName}</bdi>
                </p>
              </div>
              <div className="flex items-center gap-3">
                <HistoryStatus item={item} />
                <span className="text-sm text-muted-foreground">
                  {item.source === "qr" ? t("source.qr") : t("source.manual")}
                </span>
              </div>
            </li>
          ))}
        </ul>
      )}
      {query.hasNextPage && !query.isError && (
        <LoadMoreButton
          pending={query.isFetchingNextPage}
          onClick={() => {
            void query.fetchNextPage();
          }}
        />
      )}
    </section>
  );
}
