import { CalendarOff, CalendarSync } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetTimetable, useListGroups, type TimetableSessionDto } from "@/api/generated/tutoring-centre";
import { EmptyState } from "@/components/shared/EmptyState";
import { FilterBar } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { CancelSessionDialog } from "@/features/schedule/CancelSessionDialog";
import { todayIn } from "@/features/schedule/centreTime";
import { ExtendScheduleDialog } from "@/features/schedule/ExtendScheduleDialog";
import { TimetableGrid } from "@/features/schedule/TimetableGrid";
import { WeekNavigator } from "@/features/schedule/WeekNavigator";
import { startOfWeek } from "@/features/schedule/week";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";
import { useCentreTimeZone } from "@/features/session/useCentreTimeZone";
import { TeacherSelect } from "@/features/teachers/TeacherSelect";

export interface TimetableSearchPatch {
  week?: string;
  group?: string | undefined;
  teacher?: string | undefined;
}

/**
 * One week of sessions in the centre's time, the same page for every role: the server returns a teacher only
 * their own sessions, so this page holds no role check. The week and both filters live in the URL. Every time is
 * formatted from its UTC instant in the response's `timeZoneId`; "today" and the current week come from the
 * centre's zone, never the browser's. Manage actions sit inside `schedule.manage`; the teacher filter (and its
 * request to /api/teachers) exists only with `teachers.view`.
 */
export function TimetablePage({
  week,
  groupId,
  teacherId,
  onSearchChange,
}: {
  week: string | undefined;
  groupId: string | undefined;
  teacherId: string | undefined;
  onSearchChange: (patch: TimetableSearchPatch) => void;
}) {
  const { t } = useTranslation("schedule");
  const zone = useCentreTimeZone();
  const today = zone === null ? null : todayIn(zone);
  const currentWeekStart = today === null ? null : startOfWeek(today);
  const weekStart = week ?? currentWeekStart;
  const [extendOpen, setExtendOpen] = useState(false);

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("timetable.title")}
        description={t("timetable.subtitle")}
        actions={
          <Can permission={Permissions.ScheduleManage}>
            <Button
              type="button"
              variant="outline"
              disabled={today === null}
              onClick={() => {
                setExtendOpen(true);
              }}
            >
              <CalendarSync className="size-4" aria-hidden="true" />
              {t("actions.extend")}
            </Button>
          </Can>
        }
      />

      <FilterBar label={t("timetable.filtersLabel")}>
        <Can permission={Permissions.GroupsView}>
          <div className="w-full space-y-1.5 sm:w-56">
            <Label htmlFor="timetable-filter-group">{t("timetable.groupFilter")}</Label>
            <GroupFilterSelect
              value={groupId ?? null}
              onChange={(next) => {
                onSearchChange({ group: next ?? undefined });
              }}
            />
          </div>
        </Can>
        <Can permission={Permissions.TeachersView}>
          <div className="w-full space-y-1.5 sm:w-56">
            <Label htmlFor="timetable-filter-teacher">{t("timetable.teacherFilter")}</Label>
            <TeacherSelect
              id="timetable-filter-teacher"
              value={teacherId ?? null}
              allLabel={t("timetable.allTeachers")}
              onChange={(next) => {
                onSearchChange({ teacher: next ?? undefined });
              }}
            />
          </div>
        </Can>
        {weekStart !== null && (
          <div className="ms-auto">
            <WeekNavigator
              weekStart={weekStart}
              currentWeekStart={currentWeekStart}
              onWeekChange={(next) => {
                onSearchChange({ week: next });
              }}
            />
          </div>
        )}
      </FilterBar>

      {weekStart !== null && <TimetableWeek weekStart={weekStart} today={today} groupId={groupId} teacherId={teacherId} />}

      {today !== null && (
        <Can permission={Permissions.ScheduleManage}>
          <ExtendScheduleDialog today={today} open={extendOpen} onOpenChange={setExtendOpen} />
        </Can>
      )}
    </section>
  );
}

function TimetableWeek({
  weekStart,
  today,
  groupId,
  teacherId,
}: {
  weekStart: string;
  today: string | null;
  groupId: string | undefined;
  teacherId: string | undefined;
}) {
  const { t } = useTranslation("schedule");
  const { t: tCommon } = useTranslation("common");
  const canManage = useCan(Permissions.ScheduleManage);
  const { data, error, isPending, refetch } = useGetTimetable({ weekStart, groupId, teacherId });
  const [cancelTarget, setCancelTarget] = useState<TimetableSessionDto | null>(null);

  if (isPending) {
    return (
      <div role="status" aria-label={t("timetable.loadingLabel")} className="grid gap-3 lg:grid-cols-7 lg:gap-2">
        {Array.from({ length: 7 }, (_, index) => (
          <Skeleton key={index} className="h-28 w-full lg:h-64" />
        ))}
      </div>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("timetable.errorTitle")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError)}</p>
          {apiError.correlationId !== undefined && <p>{t("rules.errorReference", { correlationId: apiError.correlationId })}</p>}
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
    );
  }

  if (data.sessions.length === 0) {
    return (
      <EmptyState
        icon={CalendarOff}
        title={t("timetable.emptyTitle")}
        description={t(canManage ? "timetable.emptyManageHint" : "timetable.emptyDescription")}
      />
    );
  }

  // Only a session that is still scheduled and has not started can be cancelled; comparing instants needs no zone.
  const cancelActionFor = canManage
    ? (session: TimetableSessionDto) =>
        session.status === "scheduled" && Date.parse(session.startsAt) > Date.now()
          ? () => {
              setCancelTarget(session);
            }
          : undefined
    : undefined;

  return (
    <>
      <TimetableGrid
        weekStart={data.weekStart}
        today={today}
        timeZoneId={data.timeZoneId}
        sessions={data.sessions}
        cancelActionFor={cancelActionFor}
      />
      <Can permission={Permissions.ScheduleManage}>
        <CancelSessionDialog
          session={cancelTarget}
          timeZoneId={data.timeZoneId}
          onClose={() => {
            setCancelTarget(null);
          }}
        />
      </Can>
    </>
  );
}

const AllGroupsValue = "__all";

/** The centre's active groups (a teacher's own, since the API scopes the list), with an "All groups" entry. */
function GroupFilterSelect({ value, onChange }: { value: string | null; onChange: (value: string | null) => void }) {
  const { t } = useTranslation("schedule");
  const { data, isPending } = useListGroups();
  const groups = data?.items ?? [];
  const nameOf = (id: string) => groups.find((group) => group.id === id)?.name ?? "";

  return (
    <Select
      value={value ?? AllGroupsValue}
      onValueChange={(next: string | null) => {
        onChange(next === null || next === AllGroupsValue ? null : next);
      }}
      disabled={isPending}
    >
      <SelectTrigger id="timetable-filter-group" className="w-full">
        <SelectValue>
          {(selected: string | null) =>
            selected === null || selected === AllGroupsValue ? t("timetable.allGroups") : <bdi>{nameOf(selected)}</bdi>
          }
        </SelectValue>
      </SelectTrigger>
      <SelectContent>
        <SelectItem value={AllGroupsValue}>{t("timetable.allGroups")}</SelectItem>
        {groups.map((group) => (
          <SelectItem key={group.id} value={group.id}>
            <bdi>{group.name}</bdi>
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
