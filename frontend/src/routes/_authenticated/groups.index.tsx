import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { CreateGroupDialog } from "@/features/groups/CreateGroupDialog";
import { GroupsList } from "@/features/groups/GroupsList";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";
import { SubjectSelect } from "@/features/subjects/SubjectSelect";
import { TeacherSelect } from "@/features/teachers/TeacherSelect";

export interface GroupsSearch {
  subject?: string;
  teacher?: string;
  archived?: boolean;
}

export const Route = createFileRoute("/_authenticated/groups/")({
  validateSearch: (search: Record<string, unknown>): GroupsSearch => ({
    ...(typeof search.subject === "string" && search.subject !== "" ? { subject: search.subject } : {}),
    ...(typeof search.teacher === "string" && search.teacher !== "" ? { teacher: search.teacher } : {}),
    ...(search.archived === true || search.archived === "true" ? { archived: true } : {}),
  }),
  component: GroupsPage,
});

function GroupsPage() {
  return (
    <RouteGuard permission={Permissions.GroupsView}>
      <GroupsPageContent />
    </RouteGuard>
  );
}

/**
 * One page for owners, secretaries and teachers: the API scopes the rows. The teacher filter is rendered only
 * for a session with `teachers.view` — without it the select would call `GET /api/teachers` and get a 403 on
 * every load — and the create button only with `groups.manage`.
 */
function GroupsPageContent() {
  const { t } = useTranslation("groups");
  const { subject, teacher, archived = false } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  const [createOpen, setCreateOpen] = useState(false);

  const updateSearch = (patch: Partial<GroupsSearch>) => {
    void navigate({ search: (previous) => ({ ...previous, ...patch }), replace: true });
  };

  const openCreate = () => {
    setCreateOpen(true);
  };

  return (
    <section className="space-y-6">
      <div className="flex flex-wrap items-end justify-between gap-4">
        <div className="space-y-1">
          <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>
          <p className="text-sm text-muted-foreground">{t("page.subtitle")}</p>
        </div>
        <Can permission={Permissions.GroupsManage}>
          <Button type="button" onClick={openCreate}>
            <Plus className="size-4" aria-hidden="true" />
            {t("actions.create")}
          </Button>
        </Can>
      </div>

      <div
        role="search"
        aria-label={t("filters.label")}
        className="flex flex-wrap items-end gap-4 rounded-xl border border-border bg-card p-4"
      >
        <div className="w-full space-y-1.5 sm:w-56">
          <Label htmlFor="groups-filter-subject">{t("filters.subject")}</Label>
          <SubjectSelect
            id="groups-filter-subject"
            value={subject ?? null}
            allLabel={t("filters.allSubjects")}
            onChange={(next) => {
              updateSearch({ subject: next ?? undefined });
            }}
          />
        </div>
        <Can permission={Permissions.TeachersView}>
          <div className="w-full space-y-1.5 sm:w-56">
            <Label htmlFor="groups-filter-teacher">{t("filters.teacher")}</Label>
            <TeacherSelect
              id="groups-filter-teacher"
              value={teacher ?? null}
              allLabel={t("filters.allTeachers")}
              onChange={(next) => {
                updateSearch({ teacher: next ?? undefined });
              }}
            />
          </div>
        </Can>
        <div className="flex min-h-8 items-center gap-2">
          <Switch
            id="groups-show-archived"
            checked={archived}
            onCheckedChange={(next) => {
              updateSearch({ archived: next ? true : undefined });
            }}
          />
          <Label htmlFor="groups-show-archived">{t("filters.showArchived")}</Label>
        </div>
      </div>

      <GroupsList subjectId={subject} teacherId={teacher} includeArchived={archived} onCreate={openCreate} />

      <Can permission={Permissions.GroupsManage}>
        <CreateGroupDialog open={createOpen} onOpenChange={setCreateOpen} />
      </Can>
    </section>
  );
}
