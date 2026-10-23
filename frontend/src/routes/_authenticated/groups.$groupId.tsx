import { useQueryClient } from "@tanstack/react-query";
import { Link, createFileRoute, useNavigate } from "@tanstack/react-router";
import {
  Archive,
  ArchiveRestore,
  ArrowLeft,
  BookOpen,
  CalendarDays,
  LayoutGrid,
  Pencil,
  Presentation,
  SearchX,
  Users,
  UsersRound,
} from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import {
  getGetGroupQueryKey,
  getListGroupsQueryKey,
  useArchiveGroup,
  useGetGroup,
  useRestoreGroup,
  type GroupDto,
} from "@/api/generated/tutoring-centre";
import { StatusBadge } from "@/components/shared/StatusBadge";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { AssignGroupTeacherDialog, ChangeGroupCapacityDialog, RenameGroupDialog } from "@/features/groups/GroupEditDialogs";
import { SeatsMeter } from "@/features/groups/SeatsMeter";
import { showGroupError } from "@/features/groups/showGroupError";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { RouteGuard } from "@/features/session/RouteGuard";

const GroupTabs = ["overview", "roster", "schedule"] as const;
type GroupTab = (typeof GroupTabs)[number];

export interface GroupDetailSearch {
  tab: GroupTab;
}

function isGroupTab(value: unknown): value is GroupTab {
  return typeof value === "string" && (GroupTabs as readonly string[]).includes(value);
}

export const Route = createFileRoute("/_authenticated/groups/$groupId")({
  validateSearch: (search: Record<string, unknown>): GroupDetailSearch => ({
    tab: isGroupTab(search.tab) ? search.tab : "overview",
  }),
  component: GroupDetailPage,
});

function GroupDetailPage() {
  return (
    <RouteGuard permission={Permissions.GroupsView}>
      <GroupDetailPageContent />
    </RouteGuard>
  );
}

function GroupDetailPageContent() {
  const { groupId } = Route.useParams();
  const { t } = useTranslation("groups");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useGetGroup(groupId);

  if (isPending) {
    return (
      <section className="space-y-4">
        <div role="status" aria-label={t("loading.detailLabel")} className="space-y-4">
          <Skeleton className="h-8 w-64" />
          <Skeleton className="h-28 w-full" />
          <Skeleton className="h-10 w-72" />
          <Skeleton className="h-40 w-full" />
        </div>
      </section>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    if (apiError.kind === "notFound") {
      return <GroupNotFound />;
    }

    return (
      <section className="space-y-4">
        <Alert variant="destructive">
          <AlertTitle>{t("detailError.title")}</AlertTitle>
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
      </section>
    );
  }

  return <GroupDetailContent group={data} />;
}

/**
 * The one state for every reason a group is unavailable: an unknown ID, another centre's group, or — for a
 * teacher — a group taught by someone else. The API answers all three with the same 404, and the page adds
 * nothing that could tell them apart.
 */
function GroupNotFound() {
  const { t } = useTranslation("groups");

  return (
    <div className="flex flex-col items-center gap-3 py-16 text-center">
      <span className="flex size-14 items-center justify-center rounded-full bg-muted text-muted-foreground">
        <SearchX className="size-7" aria-hidden="true" />
      </span>
      <h1 className="text-xl font-semibold tracking-tight">{t("notFound.title")}</h1>
      <p className="max-w-sm text-sm text-muted-foreground">{t("notFound.description")}</p>
      <Button render={<Link to="/groups" />}>{t("notFound.backToList")}</Button>
    </div>
  );
}

type EditDialog = "rename" | "capacity" | "teacher" | null;

function GroupDetailContent({ group }: { group: GroupDto }) {
  const { t } = useTranslation("groups");
  const { tab } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  const queryClient = useQueryClient();
  const archiveMutation = useArchiveGroup();
  const restoreMutation = useRestoreGroup();
  const [editDialog, setEditDialog] = useState<EditDialog>(null);
  const [archiveConfirmOpen, setArchiveConfirmOpen] = useState(false);
  const isArchived = group.status === "archived";

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetGroupQueryKey(group.id) }),
      queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() }),
    ]);

  const handleArchiveConfirm = async () => {
    try {
      await archiveMutation.mutateAsync({ id: group.id, data: { version: group.version } });
      toast.success(t("toasts.archived"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      showGroupError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    } finally {
      await invalidate();
      setArchiveConfirmOpen(false);
    }
  };

  const handleRestore = async () => {
    try {
      await restoreMutation.mutateAsync({ id: group.id, data: { version: group.version } });
      toast.success(t("toasts.restored"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      showGroupError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    } finally {
      await invalidate();
    }
  };

  const closeEditDialog = (open: boolean) => {
    if (!open) {
      setEditDialog(null);
    }
  };

  return (
    <section className="space-y-6">
      <Link
        to="/groups"
        className="inline-flex items-center gap-1.5 text-sm text-muted-foreground underline-offset-4 hover:text-foreground hover:underline"
      >
        <ArrowLeft className="size-4 rtl:rotate-180" aria-hidden="true" />
        {t("detail.backToList")}
      </Link>

      <div className="rounded-xl border border-border bg-card p-5">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div className="min-w-0 space-y-2">
            <div className="flex flex-wrap items-center gap-3">
              <h1 className="text-2xl font-semibold tracking-tight" dir="auto">
                {group.name}
              </h1>
              <StatusBadge status={group.status} />
            </div>
            <p className="flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-muted-foreground">
              <span className="inline-flex items-center gap-1.5">
                <BookOpen className="size-4" aria-hidden="true" />
                <span dir="auto">{group.subjectName}</span>
              </span>
              <span className="inline-flex items-center gap-1.5">
                <Presentation className="size-4" aria-hidden="true" />
                <span dir="auto">{group.teacherName}</span>
              </span>
            </p>
          </div>

          <Can permission={Permissions.GroupsManage}>
            <div className="flex flex-wrap items-center gap-2">
              {isArchived ? (
                <Button
                  type="button"
                  variant="outline"
                  disabled={restoreMutation.isPending}
                  onClick={() => {
                    void handleRestore();
                  }}
                >
                  <ArchiveRestore className="size-4" aria-hidden="true" />
                  {t("actions.restore")}
                </Button>
              ) : (
                <>
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => {
                      setEditDialog("rename");
                    }}
                  >
                    <Pencil className="size-4" aria-hidden="true" />
                    {t("actions.rename")}
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => {
                      setEditDialog("capacity");
                    }}
                  >
                    {t("actions.changeCapacity")}
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => {
                      setEditDialog("teacher");
                    }}
                  >
                    {t("actions.changeTeacher")}
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    className="text-destructive hover:text-destructive"
                    onClick={() => {
                      setArchiveConfirmOpen(true);
                    }}
                  >
                    <Archive className="size-4" aria-hidden="true" />
                    {t("actions.archive")}
                  </Button>
                </>
              )}
            </div>
          </Can>
        </div>
      </div>

      {isArchived && (
        <Alert>
          <AlertTitle>{t("archivedBanner.title")}</AlertTitle>
          <AlertDescription>{t("archivedBanner.description")}</AlertDescription>
        </Alert>
      )}

      <Tabs
        value={tab}
        onValueChange={(next) => {
          if (isGroupTab(next)) {
            void navigate({ search: { tab: next }, replace: true });
          }
        }}
      >
        <TabsList aria-label={t("tabs.label")}>
          <TabsTrigger value="overview">
            <LayoutGrid aria-hidden="true" />
            {t("tabs.overview")}
          </TabsTrigger>
          <TabsTrigger value="roster">
            <Users aria-hidden="true" />
            {t("tabs.roster")}
          </TabsTrigger>
          <TabsTrigger value="schedule">
            <CalendarDays aria-hidden="true" />
            {t("tabs.schedule")}
          </TabsTrigger>
        </TabsList>

        <TabsContent value="overview">
          <div className="grid gap-4 md:grid-cols-3">
            <div className="rounded-xl border border-border bg-card p-4 md:col-span-1">
              <p className="mb-3 text-sm text-muted-foreground">{t("overview.seats")}</p>
              <SeatsMeter enrolled={Number(group.enrolledCount)} capacity={Number(group.capacity)} size="lg" />
            </div>
            <dl className="grid gap-4 rounded-xl border border-border bg-card p-4 sm:grid-cols-2 md:col-span-2">
              <div className="space-y-1">
                <dt className="text-sm text-muted-foreground">{t("overview.subject")}</dt>
                <dd className="font-medium">
                  <span dir="auto">{group.subjectName}</span>
                </dd>
              </div>
              <div className="space-y-1">
                <dt className="text-sm text-muted-foreground">{t("overview.teacher")}</dt>
                <dd className="font-medium">
                  <span dir="auto">{group.teacherName}</span>
                </dd>
              </div>
              <div className="space-y-1">
                <dt className="text-sm text-muted-foreground">{t("overview.capacity")}</dt>
                <dd className="font-medium tabular-nums">
                  <bdi>{String(Number(group.capacity))}</bdi>
                </dd>
              </div>
              <div className="space-y-1">
                <dt className="text-sm text-muted-foreground">{t("overview.status")}</dt>
                <dd>
                  <StatusBadge status={group.status} />
                </dd>
              </div>
            </dl>
          </div>
        </TabsContent>

        <TabsContent value="roster">
          <TabPlaceholder icon={UsersRound} title={t("placeholders.rosterTitle")} description={t("placeholders.rosterDescription")} />
        </TabsContent>

        <TabsContent value="schedule">
          <TabPlaceholder
            icon={CalendarDays}
            title={t("placeholders.scheduleTitle")}
            description={t("placeholders.scheduleDescription")}
          />
        </TabsContent>
      </Tabs>

      <Can permission={Permissions.GroupsManage}>
        <RenameGroupDialog group={group} open={editDialog === "rename"} onOpenChange={closeEditDialog} />
        <ChangeGroupCapacityDialog group={group} open={editDialog === "capacity"} onOpenChange={closeEditDialog} />
        {editDialog === "teacher" && <AssignGroupTeacherDialog group={group} open onOpenChange={closeEditDialog} />}

        <AlertDialog open={archiveConfirmOpen} onOpenChange={setArchiveConfirmOpen}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle dir="auto">{t("confirmations.archiveTitle", { name: group.name })}</AlertDialogTitle>
              <AlertDialogDescription>{t("confirmations.archiveBody")}</AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>{t("form.cancel")}</AlertDialogCancel>
              <AlertDialogAction
                disabled={archiveMutation.isPending}
                onClick={() => {
                  void handleArchiveConfirm();
                }}
              >
                {t("confirmations.archiveConfirm")}
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </Can>
    </section>
  );
}

function TabPlaceholder({
  icon: Icon,
  title,
  description,
}: {
  icon: typeof UsersRound;
  title: string;
  description: string;
}) {
  return (
    <div className="flex flex-col items-center gap-2 rounded-xl border border-dashed border-border px-6 py-12 text-center">
      <Icon className="size-8 text-muted-foreground" aria-hidden="true" />
      <p className="text-sm font-semibold">{title}</p>
      <p className="max-w-sm text-sm text-muted-foreground">{description}</p>
    </div>
  );
}
