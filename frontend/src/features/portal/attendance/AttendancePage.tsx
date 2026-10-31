import { useState } from "react";
import { useTranslation } from "react-i18next";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { useGetAttendanceInfinite } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { useSession } from "@/features/session/useSession";
import { useCentreTimeZone } from "@/features/session/useCentreTimeZone";
import { formatTime, todayIn } from "@/features/schedule/centreTime";
import { HistoryStatus } from "@/features/attendance/HistoryStatus";
import { PageHeader } from "@/components/shared/PageHeader";
import { Route } from "@/routes/portal.attendance";

export function AttendancePage() {
  const { t, i18n } = useTranslation("portal");
  const { t: attendance } = useTranslation("attendance");
  const { me } = useSession();
  const zone = useCentreTimeZone();
  const search = Route.useSearch();
  const navigate = Route.useNavigate();
  const month = search.month ?? (zone ? todayIn(zone).slice(0, 7) : "");
  const children = me?.portal?.children ?? [];
  const [selected, select] = useState<string>();
  const child = children.find((item) => item.id === selected) ?? children[0];
  const [year = 2000, number = 1] = month.split("-").map(Number);
  const end = month ? new Date(Date.UTC(year, number, 0)).toISOString().slice(0, 10) : undefined;
  const query = useGetAttendanceInfinite(
    { studentId: child?.id, from: month ? `${month}-01` : undefined, to: end },
    {
      query: {
        enabled: !!zone && !!month && !!child,
        initialPageParam: undefined,
        getNextPageParam: (page) => page.nextCursor ?? undefined,
      },
    },
  );
  const rows = query.data?.pages.flatMap((page) => page.items) ?? [];
  const dates = [...new Set(rows.map((row) => row.localDate))];
  const move = (delta: number) => {
    const next = new Date(Date.UTC(year, number - 1 + delta, 1)).toISOString().slice(0, 7);
    void navigate({ search: { month: next } });
  };
  return (
    <section className="mx-auto max-w-3xl space-y-7">
      <PageHeader title={t("attendance")} />
      <div className="flex flex-wrap items-end justify-between gap-5 rounded-xl border bg-card p-4">
        {me?.portal?.kind === "parent" && children.length > 1 && (
          <label className="grid gap-2 text-sm font-medium">
            {t("attendanceChild")}
            <select
              className="h-10 rounded-md border bg-background px-3 text-sm"
              value={child?.id ?? ""}
              onChange={(event) => {
                select(event.target.value);
              }}
            >
              {children.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.name}
                </option>
              ))}
            </select>
          </label>
        )}
        <div className="flex items-center gap-4">
          <Button
            variant="outline"
            aria-label={t("attendancePrevious")}
            onClick={() => {
              move(-1);
            }}
            disabled={!month}
          >
            <ChevronLeft className="size-4 rtl:rotate-180" />
          </Button>
          <span className="min-w-36 text-center font-medium">
            {month
              ? new Intl.DateTimeFormat(i18n.language, {
                  month: "long",
                  year: "numeric",
                  timeZone: "UTC",
                }).format(new Date(`${month}-01T12:00:00Z`))
              : ""}
          </span>
          <Button
            variant="outline"
            aria-label={t("attendanceNext")}
            onClick={() => {
              move(1);
            }}
            disabled={!month}
          >
            <ChevronRight className="size-4 rtl:rotate-180" />
          </Button>
        </div>
      </div>
      {children.length > 0 && query.isPending && <p role="status">{t("attendanceLoading")}</p>}
      {query.isError && (
        <div role="alert">
          {t("attendanceError")}{" "}
          <Button
            onClick={() => {
              void query.refetch();
            }}
          >
            {t("attendanceRetry")}
          </Button>
        </div>
      )}
      {(children.length === 0 || (query.isSuccess && rows.length === 0)) && (
        <p className="border-y py-12 text-center text-muted-foreground">{t("attendanceEmpty")}</p>
      )}
      {dates.map((date) => (
        <section key={date} className="space-y-3">
          <h2 className="text-sm font-semibold">
            <time dateTime={date}>{date}</time>
          </h2>
          <ul className="divide-y overflow-hidden rounded-xl border bg-card px-4">
            {rows
              .filter((row) => row.localDate === date)
              .map((row) => (
                <li
                  key={row.recordId}
                  className="flex flex-wrap items-center justify-between gap-4 py-4"
                >
                  <div>
                    <p className="font-medium" dir="auto">
                      {row.groupName}
                    </p>
                    <time className="text-sm text-muted-foreground" dateTime={row.startsAt}>
                      {zone ? formatTime(row.startsAt, zone, i18n.language) : ""}
                    </time>
                  </div>
                  <span className="flex items-center gap-2 text-sm">
                    <HistoryStatus item={row} />
                    {attendance(`status.${row.status}`)}
                  </span>
                </li>
              ))}
          </ul>
        </section>
      ))}
      {query.hasNextPage && (
        <Button
          variant="outline"
          disabled={query.isFetchingNextPage}
          onClick={() => {
            void query.fetchNextPage();
          }}
        >
          {t("attendanceMore")}
        </Button>
      )}
    </section>
  );
}
