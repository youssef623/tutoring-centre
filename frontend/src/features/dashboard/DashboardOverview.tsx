import {
  DataTable,
  DataTableHead,
  DataTableColumn,
  DataTableBody,
  DataTableRow,
  DataTableCell,
} from "@/components/shared/DataTable";
import { Link } from "@tanstack/react-router";
import { BookOpen, Armchair, ArrowRight, GraduationCap, UserPlus, UsersRound } from "lucide-react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListGroups, type GroupDto } from "@/api/generated/tutoring-centre";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { Skeleton } from "@/components/ui/skeleton";
import { SeatMeter } from "@/features/enrollments/SeatMeter";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";

/** A group counts as "filling up" from this share of its seats taken; the same threshold as the seat meter's badge. */
const FillingUpShare = 0.8;
const FillingUpLimit = 5;

/** The shortcuts a session may use, each behind its own permission; nothing renders for a session with none. */
export function QuickActions() {
  const { t } = useTranslation("shell");

  return (
    <div className="quick-actions flex flex-wrap gap-3">
      <Can permission={Permissions.StudentsManage}>
        <Link to="/students/new" className={buttonVariants()}>
          <UserPlus className="size-4" aria-hidden="true" />
          {t("dashboard.actions.registerStudent")}
        </Link>
      </Can>
      <Can permission={Permissions.StudentsView}>
        <Link to="/students" className={buttonVariants({ variant: "outline" })}>
          <GraduationCap className="size-4" aria-hidden="true" />
          {t("dashboard.actions.students")}
        </Link>
      </Can>
      <Can permission={Permissions.GroupsView}>
        <Link to="/groups" className={buttonVariants({ variant: "outline" })}>
          <UsersRound className="size-4" aria-hidden="true" />
          {t("dashboard.actions.groups")}
        </Link>
      </Can>
    </div>
  );
}

/**
 * Seats across the centre's active groups (for a teacher, the API has already narrowed them to their own), and
 * the groups closest to full. Every figure is summed from the server's own `enrolledCount` and `capacity`.
 * Rendered only with `groups.view`, so without it no request is sent.
 */
export function GroupsOverview() {
  const { t } = useTranslation("shell");
  const { t: tCommon } = useTranslation("common");
  const { t: tGroups } = useTranslation("groups");
  const { data, error, isPending, refetch } = useListGroups();

  if (isPending) {
    return (
      <div role="status" aria-label={t("dashboard.loadingLabel")} className="space-y-4">
        <div className="grid grid-cols-2 gap-2 sm:grid-cols-4 sm:gap-3">
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
        </div>
        <Skeleton className="h-48 w-full" />
      </div>
    );
  }

  if (error) {
    return (
      <Alert variant="destructive">
        <AlertTitle>{t("dashboard.errorTitle")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(asApiError(error))}</p>
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

  const active = data.items.filter((group) => group.status === "active");
  const seatsTaken = active.reduce((sum, group) => sum + Number(group.enrolledCount), 0);
  const capacity = active.reduce((sum, group) => sum + Number(group.capacity), 0);
  const share = (group: GroupDto) =>
    Number(group.enrolledCount) / Math.max(1, Number(group.capacity));
  const fullCount = active.filter((group) => share(group) >= 1).length;
  const fillingUp = active
    .filter((group) => share(group) >= FillingUpShare)
    .sort((a, b) => share(b) - share(a) || a.name.localeCompare(b.name))
    .slice(0, FillingUpLimit);

  return (
    <div className="space-y-6">
      <section className="capacity-ribbon">
        <div className="capacity-ribbon-title">
          <Armchair aria-hidden="true" />
          <div>
            <h2>{t("dashboard.capacity")}</h2>
            <p>{t("dashboard.capacityDescription")}</p>
          </div>
        </div>
        <div className="capacity-ribbon-data">
          <div className="capacity-distribution" aria-hidden="true">
            <span
              className={`share-${String(Math.round((seatsTaken / Math.max(1, capacity)) * 100))}`}
            />
            <span
              className={`share-${String(Math.round((Math.max(0, capacity - seatsTaken) / Math.max(1, capacity)) * 100))}`}
            />
            <span className={capacity === 0 ? "share-100" : "share-0"} />
          </div>
          <dl>
            {[
              ["activeGroups", active.length],
              ["seatsTaken", seatsTaken],
              ["openSeats", Math.max(0, capacity - seatsTaken)],
              ["fullGroups", fullCount],
            ].map(([key, value]) => (
              <div key={key}>
                <dt>{t(`dashboard.stats.${String(key)}`)}</dt>
                <dd>{value}</dd>
              </div>
            ))}
          </dl>
        </div>
      </section>
      <div className="dashboard-lower">
        <section
          aria-labelledby="dashboard-filling-up"
          className="capacity-surface rounded-xl border border-border bg-card"
        >
          <div className="flex flex-wrap items-center justify-between gap-2 border-b border-border px-4 py-3">
            <div className="space-y-0.5">
              <h2 id="dashboard-filling-up" className="text-base font-semibold">
                {t("dashboard.fillingUp.title")}
              </h2>
              <p className="text-sm text-muted-foreground">
                {t("dashboard.fillingUp.description")}
              </p>
            </div>
            <Link
              to="/groups"
              className="inline-flex items-center gap-1 text-sm font-medium text-primary underline-offset-4 hover:underline"
            >
              {t("dashboard.fillingUp.viewAll")}
              <ArrowRight className="size-4 rtl:rotate-180" aria-hidden="true" />
            </Link>
          </div>

          {fillingUp.length === 0 ? (
            <p className="dashboard-empty">
              <BookOpen aria-hidden="true" />
              {active.length === 0
                ? t("dashboard.fillingUp.noGroups")
                : t("dashboard.fillingUp.empty")}
            </p>
          ) : (
            <DataTable>
              <DataTableHead>
                <DataTableColumn>{tGroups("columns.name")}</DataTableColumn>
                <DataTableColumn>{tGroups("columns.subject")}</DataTableColumn>
                <DataTableColumn>{tGroups("columns.teacher")}</DataTableColumn>
                <DataTableColumn>{tGroups("columns.seats")}</DataTableColumn>
              </DataTableHead>
              <DataTableBody>
                {fillingUp.map((group) => (
                  <DataTableRow key={group.id}>
                    <DataTableCell kind="primary">
                      <Link
                        to="/groups/$groupId"
                        params={{ groupId: group.id }}
                        search={{ tab: "roster" }}
                        className="font-medium hover:text-primary"
                      >
                        <bdi>{group.name}</bdi>
                      </Link>
                    </DataTableCell>
                    <DataTableCell label={tGroups("columns.subject")}>
                      <bdi>{group.subjectName}</bdi>
                    </DataTableCell>
                    <DataTableCell label={tGroups("columns.teacher")}>
                      <bdi>{group.teacherName}</bdi>
                    </DataTableCell>
                    <DataTableCell label={tGroups("columns.seats")}>
                      <SeatMeter
                        enrolled={Number(group.enrolledCount)}
                        capacity={Number(group.capacity)}
                      />
                    </DataTableCell>
                  </DataTableRow>
                ))}
              </DataTableBody>
            </DataTable>
          )}
        </section>
        <aside className="dashboard-quick">
          <h2>{t("dashboard.quickActions")}</h2>
          <QuickActions />
        </aside>
      </div>
    </div>
  );
}
