import { useInfiniteQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import {
  getListStudentsQueryKey,
  listStudents,
  type ListStudentsParams,
  type StudentListItemDto,
} from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { LoadMoreButton } from "@/components/shared/LoadMoreButton";
import { PhoneText } from "@/components/shared/PhoneText";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";
import { isolateLtr } from "@/lib/bidi";

const PageSize = 25;

export function StudentsList({ search, includeArchived }: { search: string; includeArchived: boolean }) {
  const { t } = useTranslation("students");
  const { t: tCommon } = useTranslation("common");
  const canManage = useCan(Permissions.StudentsManage);

  const params: ListStudentsParams = {
    ...(search === "" ? {} : { search }),
    includeArchived,
    pageSize: PageSize,
  };

  const { data, error, isPending, refetch, fetchNextPage, hasNextPage, isFetchingNextPage } = useInfiniteQuery({
    queryKey: getListStudentsQueryKey(params),
    queryFn: ({ pageParam, signal }) => listStudents({ ...params, cursor: pageParam }, { signal }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
  });

  if (isPending) {
    return (
      <div role="status" aria-label={t("loading.label")} className="space-y-2">
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
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
          {apiError.correlationId !== undefined && <p>{t("error.reference", { correlationId: isolateLtr(apiError.correlationId) })}</p>}
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

  const items = data.pages.flatMap((page) => page.items);

  if (items.length === 0) {
    if (search === "") {
      return (
        <div className="rounded-xl border border-dashed border-border py-10 text-center">
          <p className="text-sm font-medium">{t("emptyState.title")}</p>
          <p className="text-sm text-muted-foreground">{t("emptyState.description")}</p>
          {canManage && (
            <Button render={<Link to="/students/new" />} className="mt-3">
              {t("actions.register")}
            </Button>
          )}
        </div>
      );
    }

    return (
      <div className="rounded-xl border border-dashed border-border py-10 text-center">
        <p className="text-sm font-medium" dir="auto">
          {t("emptyState.noMatchTitle", { search })}
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      <DataTable className="sm:min-w-[640px]">
        <DataTableHead>
          <DataTableColumn>{t("columns.name")}</DataTableColumn>
          <DataTableColumn>{t("columns.schoolYear")}</DataTableColumn>
          <DataTableColumn>{t("columns.primaryParent")}</DataTableColumn>
          <DataTableColumn>{t("columns.parentPhone")}</DataTableColumn>
        </DataTableHead>
        <DataTableBody>
          {items.map((student) => (
            <StudentRow key={student.id} student={student} />
          ))}
        </DataTableBody>
      </DataTable>

      {hasNextPage && (
        <LoadMoreButton
          pending={isFetchingNextPage}
          onClick={() => {
            void fetchNextPage();
          }}
        />
      )}
    </div>
  );
}

function StudentRow({ student }: { student: StudentListItemDto }) {
  const { t } = useTranslation("students");
  const isArchived = student.status === "archived";

  return (
    <DataTableRow muted={isArchived}>
      <DataTableCell kind="primary">
        <span className="flex flex-wrap items-center gap-2">
          <Link to="/students/$studentId" params={{ studentId: student.id }} className="font-medium hover:underline">
            <bdi>{student.fullName}</bdi>
          </Link>
          {isArchived && <StatusBadge status="archived" />}
        </span>
      </DataTableCell>
      <DataTableCell label={t("columns.schoolYear")}>
        <bdi>{student.schoolYear ?? "—"}</bdi>
      </DataTableCell>
      <DataTableCell label={t("columns.primaryParent")}>
        <bdi>{student.primaryParentName ?? "—"}</bdi>
      </DataTableCell>
      <DataTableCell label={t("columns.parentPhone")}>
        {student.primaryParentPhone === null ? "—" : <PhoneText value={student.primaryParentPhone} />}
      </DataTableCell>
    </DataTableRow>
  );
}
