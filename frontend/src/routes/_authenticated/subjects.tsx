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
    <section className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2">
            <Switch id="show-archived" checked={archived} onCheckedChange={setArchived} />
            <Label htmlFor="show-archived">{t("page.showArchived")}</Label>
          </div>
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
        </div>
      </div>

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
