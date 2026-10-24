import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useTranslation } from "react-i18next";
import { SearchInput } from "@/components/shared/SearchInput";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { StudentsList } from "@/features/students/StudentsList";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";
import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";

export interface StudentsSearch {
  q?: string;
  archived?: boolean;
}

export const Route = createFileRoute("/_authenticated/students/")({
  validateSearch: (search: Record<string, unknown>): StudentsSearch => ({
    ...(typeof search.q === "string" && search.q !== "" ? { q: search.q } : {}),
    ...(search.archived === true || search.archived === "true" ? { archived: true } : {}),
  }),
  component: StudentsPage,
});

function StudentsPage() {
  return (
    <RouteGuard permission={Permissions.StudentsView}>
      <StudentsPageContent />
    </RouteGuard>
  );
}

function StudentsPageContent() {
  const { t } = useTranslation("students");
  const { q = "", archived = false } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });

  const setQuery = (next: string) => {
    void navigate({ search: (previous) => ({ ...previous, q: next === "" ? undefined : next }), replace: true });
  };

  const setArchived = (next: boolean) => {
    void navigate({ search: (previous) => ({ ...previous, archived: next ? true : undefined }), replace: true });
  };

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("page.title")}
        description={t("page.subtitle")}
        actions={
          <Can permission={Permissions.StudentsManage}>
            <Button render={<Link to="/students/new" />}>
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.register")}
            </Button>
          </Can>
        }
      />

      <FilterBar label={t("page.filtersLabel")}>
        <div className="w-full sm:max-w-xs">
          <SearchInput value={q} onChange={setQuery} label={t("page.searchLabel")} />
        </div>
        <FilterToggle>
          <Switch id="show-archived" checked={archived} onCheckedChange={setArchived} />
          <Label htmlFor="show-archived">{t("page.showArchived")}</Label>
        </FilterToggle>
      </FilterBar>

      <StudentsList search={q} includeArchived={archived} />
    </section>
  );
}
