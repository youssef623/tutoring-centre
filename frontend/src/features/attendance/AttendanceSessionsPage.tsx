import { CalendarX2 } from "lucide-react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListAttendanceSessions } from "@/api/generated/tutoring-centre";
import { EmptyState } from "@/components/shared/EmptyState";
import { PageHeader } from "@/components/shared/PageHeader";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { AttendanceSessionCard } from "./AttendanceSessionCard";
import { DateNavigator } from "./DateNavigator";

/**
 * The teacher's starting point. The day comes from the URL (`date`); with none, the server answers for the
 * centre's own today and says which date that was, so the browser's clock and zone never pick the day. The API
 * has already scoped the list to the actor: a teacher gets their own sessions, office staff the centre's.
 */
export function AttendanceSessionsPage({
  date,
  onDateChange,
}: {
  date: string | undefined;
  onDateChange: (date: string | undefined) => void;
}) {
  const { t } = useTranslation("attendance");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useListAttendanceSessions(date === undefined ? undefined : { date });

  return (
    <section className="space-y-6">
      <PageHeader title={t("page.title")} description={t("page.subtitle")} />

      <DateNavigator date={data?.date ?? date} isToday={date === undefined} onDateChange={onDateChange} />

      {isPending ? (
        <div role="status" aria-label={t("sessions.loading.label")} className="grid gap-3 sm:grid-cols-2">
          <Skeleton className="h-36 w-full" />
          <Skeleton className="h-36 w-full" />
          <Skeleton className="h-36 w-full" />
        </div>
      ) : error ? (
        <Alert variant="destructive">
          <AlertTitle>{t("sessions.error.title")}</AlertTitle>
          <AlertDescription className="flex flex-col gap-2">
            <p>{messageFor(asApiError(error))}</p>
            {asApiError(error).correlationId !== undefined && (
              <p>
                {t("sessions.error.reference")} <bdi>{asApiError(error).correlationId}</bdi>
              </p>
            )}
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="self-start"
              onClick={() => {
                void refetch();
              }}
            >
              {tCommon("retry")}
            </Button>
          </AlertDescription>
        </Alert>
      ) : data.items.length === 0 ? (
        <EmptyState icon={CalendarX2} title={t("sessions.empty.title")} description={t("sessions.empty.description")} />
      ) : (
        <ul className="grid gap-3 sm:grid-cols-2">
          {[...data.items]
            .sort((a, b) => Date.parse(a.startsAt) - Date.parse(b.startsAt))
            .map((session) => (
              <AttendanceSessionCard key={session.sessionId} session={session} timeZoneId={data.timeZoneId} />
            ))}
        </ul>
      )}
    </section>
  );
}
