import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { SubjectFormDialog, type SubjectDialogState } from "@/features/subjects/SubjectFormDialog";
import { SubjectsList } from "@/features/subjects/SubjectsList";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";

export interface SubjectsSearch {
  archived: boolean;
}

export const Route = createFileRoute("/_authenticated/subjects")({
  validateSearch: (search: Record<string, unknown>): SubjectsSearch => ({
    archived: search.archived === true || search.archived === "true",
  }),
  component: SubjectsPage,
});

function SubjectsPage() {
  const { t } = useTranslation("subjects");
  const { archived } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  const [dialogState, setDialogState] = useState<SubjectDialogState>(null);

  const setArchived = (next: boolean) => {
    void navigate({ search: { archived: next }, replace: true });
  };

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("page.title")}
        description={t("page.subtitle")}
        actions={
          <Can permission={Permissions.SubjectsManage}>
            <Button
              type="button"
              onClick={() => {
                setDialogState({ mode: "create" });
              }}
            >
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.create")}
            </Button>
          </Can>
        }
      />

      <FilterBar label={t("page.filtersLabel")}>
        <FilterToggle>
          <Switch id="show-archived" checked={archived} onCheckedChange={setArchived} />
          <Label htmlFor="show-archived">{t("page.showArchived")}</Label>
        </FilterToggle>
      </FilterBar>

      <SubjectsList
        includeArchived={archived}
        onRename={(subject) => {
          setDialogState({ mode: "rename", subject });
        }}
      />

      <SubjectFormDialog
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
