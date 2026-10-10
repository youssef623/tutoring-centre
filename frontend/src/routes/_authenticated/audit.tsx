import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { AuditLog, type AuditFilters } from "@/features/audit/AuditLog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

const DefaultPageSize = 50;

export interface AuditSearch {
  entityType?: "subject" | "membership" | "centre";
  from?: string;
  to?: string;
  pageSize?: number;
}

const KnownEntityTypes = new Set(["subject", "membership", "centre"]);

export const Route = createFileRoute("/_authenticated/audit")({
  validateSearch: (search: Record<string, unknown>): AuditSearch => {
    const entityType = typeof search.entityType === "string" && KnownEntityTypes.has(search.entityType)
      ? (search.entityType as AuditSearch["entityType"])
      : undefined;
    const pageSize = typeof search.pageSize === "string" ? Number(search.pageSize) : undefined;

    return {
      ...(entityType === undefined ? {} : { entityType }),
      ...(typeof search.from === "string" && search.from !== "" ? { from: search.from } : {}),
      ...(typeof search.to === "string" && search.to !== "" ? { to: search.to } : {}),
      ...(pageSize !== undefined && Number.isFinite(pageSize) && pageSize > 0 ? { pageSize } : {}),
    };
  },
  component: AuditPage,
});

function AuditPage() {
  return (
    <RouteGuard permission={Permissions.AuditView}>
      <AuditPageContent />
    </RouteGuard>
  );
}

function AuditPageContent() {
  const { t } = useTranslation("audit");
  const search = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });

  const updateSearch = (patch: Partial<AuditSearch>) => {
    void navigate({ search: { ...search, ...patch }, replace: true });
  };

  const filters: AuditFilters = {
    ...(search.entityType === undefined ? {} : { entityType: search.entityType }),
    ...(search.from === undefined ? {} : { from: search.from }),
    ...(search.to === undefined ? {} : { to: search.to }),
    pageSize: search.pageSize ?? DefaultPageSize,
  };

  return (
    <section className="space-y-4">
      <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>

      <div className="flex flex-wrap items-end gap-3">
        <div className="space-y-1.5">
          <Label htmlFor="audit-entity-type">{t("filters.entityTypeLabel")}</Label>
          <Select
            value={search.entityType ?? "all"}
            onValueChange={(value) => {
              updateSearch({ entityType: value === "all" ? undefined : (value as AuditSearch["entityType"]) });
            }}
          >
            <SelectTrigger id="audit-entity-type" className="w-44">
              <SelectValue>
                {(value: string) => (value === "all" ? t("filters.entityTypeAll") : t(`entityType.${value}`))}
              </SelectValue>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">{t("filters.entityTypeAll")}</SelectItem>
              <SelectItem value="subject">{t("entityType.subject")}</SelectItem>
              <SelectItem value="membership">{t("entityType.membership")}</SelectItem>
              <SelectItem value="centre">{t("entityType.centre")}</SelectItem>
            </SelectContent>
          </Select>
        </div>

        <div className="space-y-1.5">
          <Label htmlFor="audit-from">{t("filters.fromLabel")}</Label>
          <Input
            id="audit-from"
            type="date"
            dir="ltr"
            value={search.from ?? ""}
            onChange={(event) => {
              updateSearch({ from: event.target.value === "" ? undefined : event.target.value });
            }}
          />
        </div>

        <div className="space-y-1.5">
          <Label htmlFor="audit-to">{t("filters.toLabel")}</Label>
          <Input
            id="audit-to"
            type="date"
            dir="ltr"
            value={search.to ?? ""}
            onChange={(event) => {
              updateSearch({ to: event.target.value === "" ? undefined : event.target.value });
            }}
          />
        </div>
      </div>

      <AuditLog filters={filters} />
    </section>
  );
}
