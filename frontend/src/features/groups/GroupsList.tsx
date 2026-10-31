import { formatMoney } from "@/features/billing/money";
import { Link } from "@tanstack/react-router";
import { Armchair, CircleSlash, Plus, UsersRound } from "lucide-react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListGroups } from "@/api/generated/tutoring-centre";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { EmptyState } from "@/components/shared/EmptyState";
import { StatTile } from "@/components/shared/StatTile";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { SeatMeter } from "@/features/enrollments/SeatMeter";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";

/**
 * One list for every role: it renders whatever `GET /api/groups` returns, and the API has already scoped that
 * to the actor (a teacher gets only their own groups). Nothing here asks who the user is; the only branch is
 * on a permission — without `groups.manage` the empty state speaks to someone with assigned groups, not to
 * someone who creates them.
 */
export function GroupsList({
  subjectId,
  teacherId,
  includeArchived,
  onlyFreeSeats = false,
  onCreate,
}: {
  subjectId: string | undefined;
  teacherId: string | undefined;
  includeArchived: boolean;
  onlyFreeSeats?: boolean;
  onCreate: () => void;
}) {
  const { t, i18n } = useTranslation("groups");
  const { t: tCommon } = useTranslation("common");
  const canManage = useCan(Permissions.GroupsManage);
  const { data, error, isPending, refetch } = useListGroups({
    ...(subjectId === undefined ? {} : { subjectId }),
    ...(teacherId === undefined ? {} : { teacherId }),
    includeArchived,
  });
  const isFiltered = subjectId !== undefined || teacherId !== undefined;

  if (isPending) {
    return (
      <div role="status" aria-label={t("loading.label")} className="space-y-4">
        <div className="grid grid-cols-3 gap-2 sm:gap-3">
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
        </div>
        <Skeleton className="h-14 w-full" />
        <Skeleton className="h-14 w-full" />
        <Skeleton className="h-14 w-full" />
      </div>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("error.title")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError)}</p>
          {apiError.correlationId !== undefined && (
            <p>
              {t("error.reference")} <bdi>{apiError.correlationId}</bdi>
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
    );
  }

  if (data.items.length === 0) {
    if (isFiltered) {
      return (
        <EmptyState
          icon={CircleSlash}
          title={t("emptyState.noMatchTitle")}
          description={t("emptyState.noMatchDescription")}
        />
      );
    }

    return canManage ? (
      <EmptyState
        icon={UsersRound}
        title={t("emptyState.title")}
        description={t("emptyState.description")}
        action={
          <Can permission={Permissions.GroupsManage}>
            <Button type="button" onClick={onCreate}>
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.create")}
            </Button>
          </Can>
        }
      />
    ) : (
      <EmptyState
        icon={UsersRound}
        title={t("emptyState.assignedTitle")}
        description={t("emptyState.assignedDescription")}
      />
    );
  }

  const active = data.items.filter((group) => group.status === "active");
  const enrolled = active.reduce((sum, group) => sum + Number(group.enrolledCount), 0);
  const capacity = active.reduce((sum, group) => sum + Number(group.capacity), 0);
  const fullCount = active.filter(
    (group) => Number(group.enrolledCount) >= Number(group.capacity),
  ).length;
  // A client-side filter of the page already loaded, labelled as such; the counts themselves stay the server's.
  const rows = onlyFreeSeats
    ? active.filter((group) => Number(group.enrolledCount) < Number(group.capacity))
    : data.items;

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-3 gap-2 sm:gap-3">
        <StatTile icon={UsersRound} label={t("stats.activeGroups")} value={active.length} />
        <StatTile
          icon={Armchair}
          label={t("stats.openSeats")}
          value={Math.max(0, capacity - enrolled)}
        />
        <StatTile icon={CircleSlash} label={t("stats.fullGroups")} value={fullCount} />
      </div>

      {rows.length === 0 ? (
        <EmptyState
          icon={CircleSlash}
          title={t("emptyState.noMatchTitle")}
          description={t("emptyState.noMatchDescription")}
        />
      ) : (
        <DataTable className="sm:min-w-[720px]">
          <DataTableHead>
            <DataTableColumn>{t("columns.name")}</DataTableColumn>
            <DataTableColumn>{t("columns.subject")}</DataTableColumn>
            <DataTableColumn>{t("columns.teacher")}</DataTableColumn>
            <DataTableColumn>{t("columns.seats")}</DataTableColumn>
          </DataTableHead>
          <DataTableBody>
            {rows.map((group) => {
              const isArchived = group.status === "archived";

              return (
                <DataTableRow key={group.id} muted={isArchived}>
                  <DataTableCell kind="primary">
                    <span className="flex flex-wrap items-center gap-2">
                      <Link
                        to="/groups/$groupId"
                        params={{ groupId: group.id }}
                        search={{ tab: "overview" }}
                        className="font-medium text-foreground underline-offset-4 hover:text-primary hover:underline focus-visible:rounded-sm focus-visible:outline-none focus-visible:ring-3 focus-visible:ring-ring/50"
                      >
                        <bdi>{group.name}</bdi>
                      </Link>
                      {typeof group.monthlyFee === "string" && (
                        <span className="text-xs text-muted-foreground">
                          <bdi dir="ltr">{formatMoney(group.monthlyFee, i18n.language)}</bdi> EGP
                        </span>
                      )}
                      {isArchived && <StatusBadge status="archived" />}
                    </span>
                  </DataTableCell>
                  <DataTableCell label={t("columns.subject")}>
                    <bdi>{group.subjectName}</bdi>
                  </DataTableCell>
                  <DataTableCell label={t("columns.teacher")}>
                    <bdi>{group.teacherName}</bdi>
                  </DataTableCell>
                  <DataTableCell label={t("columns.seats")}>
                    <SeatMeter
                      enrolled={Number(group.enrolledCount)}
                      capacity={Number(group.capacity)}
                    />
                  </DataTableCell>
                </DataTableRow>
              );
            })}
          </DataTableBody>
        </DataTable>
      )}
    </div>
  );
}
