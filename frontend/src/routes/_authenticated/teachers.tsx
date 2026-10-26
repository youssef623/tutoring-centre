import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";
import { TeacherFormDialog, type TeacherDialogState } from "@/features/teachers/TeacherFormDialog";
import { TeachersList } from "@/features/teachers/TeachersList";
import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";

export interface TeachersSearch {
  archived?: boolean;
}

export const Route = createFileRoute("/_authenticated/teachers")({
  validateSearch: (search: Record<string, unknown>): TeachersSearch => ({
    ...(search.archived === true || search.archived === "true" ? { archived: true } : {}),
  }),
  component: TeachersPage,
});

function TeachersPage() {
  return (
    <RouteGuard permission={Permissions.TeachersView}>
      <TeachersPageContent />
    </RouteGuard>
  );
}

function TeachersPageContent() {
  const { t } = useTranslation("teachers");
  const { archived = false } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  const [dialogState, setDialogState] = useState<TeacherDialogState>(null);

  const setArchived = (next: boolean) => {
    void navigate({ search: (previous) => ({ ...previous, archived: next ? true : undefined }), replace: true });
  };

  const openCreate = () => {
    setDialogState({ mode: "create" });
  };

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("page.title")}
        description={t("page.subtitle")}
        actions={
          <Can permission={Permissions.TeachersManage}>
            <Button type="button" onClick={openCreate}>
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.create")}
            </Button>
          </Can>
        }
      />

      <FilterBar label={t("page.filtersLabel")}>
        <FilterToggle>
          <Switch id="teachers-show-archived" checked={archived} onCheckedChange={setArchived} />
          <Label htmlFor="teachers-show-archived">{t("page.showArchived")}</Label>
        </FilterToggle>
      </FilterBar>

      <TeachersList
        includeArchived={archived}
        onCreate={openCreate}
        onEdit={(teacher) => {
          setDialogState({ mode: "edit", teacher });
        }}
      />

      <TeacherFormDialog
        state={dialogState}
        onOpenChange={(open) => {
          if (!open) {
            setDialogState(null);
          }
        }}
      />
    </section>
  );
}
