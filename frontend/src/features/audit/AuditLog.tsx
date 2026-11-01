import {
  DataTable,
  DataTableHead,
  DataTableColumn,
  DataTableBody,
  DataTableRow,
  DataTableCell,
} from "@/components/shared/DataTable";
import { useInfiniteQuery } from "@tanstack/react-query";
import { Copy } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import {
  getAuditLog,
  getGetAuditLogQueryKey,
  type AuditEntryDto,
  type GetAuditLogParams,
} from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { formatAuditValue, type AuditEntityType } from "@/features/audit/auditFormatting";
import { isolateLtr } from "@/lib/bidi";

export interface AuditFilters {
  entityType?: AuditEntityType;
  from?: string;
  to?: string;
  pageSize: number;
}

export function AuditLog({ filters }: { filters: AuditFilters }) {
  const { t, i18n } = useTranslation("audit");
  const { t: tCommon } = useTranslation("common");
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const params: GetAuditLogParams = {
    pageSize: filters.pageSize,
    ...(filters.entityType === undefined ? {} : { entityType: filters.entityType }),
    ...(filters.from === undefined || filters.from === "" ? {} : { from: filters.from }),
    ...(filters.to === undefined || filters.to === "" ? {} : { to: filters.to }),
  };

  const { data, error, isPending, refetch, fetchNextPage, hasNextPage, isFetchingNextPage } =
    useInfiniteQuery({
      queryKey: getGetAuditLogQueryKey(params),
      queryFn: ({ pageParam, signal }) => getAuditLog({ ...params, cursor: pageParam }, { signal }),
      initialPageParam: undefined as string | undefined,
      getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
    });

  if (isPending) {
    return (
      <div role="status" aria-label={t("loading.label")} className="space-y-2">
        <Skeleton className="h-16 w-full" />
        <Skeleton className="h-16 w-full" />
        <Skeleton className="h-16 w-full" />
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
            <p>{t("error.reference", { correlationId: isolateLtr(apiError.correlationId) })}</p>
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

  const items = data.pages.flatMap((page) => page.items);
  const selected = items.find((item) => item.id === selectedId) ?? items[0];

  if (items.length === 0) {
    return (
      <div className="rounded-xl border border-dashed border-border py-10 text-center">
        <p className="text-sm font-medium">{t("emptyState.title")}</p>
      </div>
    );
  }

  return (
    <div className="audit-browser">
      <div className="audit-ledger space-y-3">
        <DataTable>
          <DataTableHead>
            {["time", "actor", "action", "entity"].map((key) => (
              <DataTableColumn key={key}>{t(`columns.${key}`)}</DataTableColumn>
            ))}
          </DataTableHead>
          <DataTableBody>
            {items.map((item) => (
              <DataTableRow key={item.id}>
                <DataTableCell label={t("columns.time")}>
                  <time dateTime={item.occurredAt}>
                    {new Intl.DateTimeFormat(i18n.language, {
                      dateStyle: "medium",
                      timeStyle: "short",
                    }).format(new Date(item.occurredAt))}
                  </time>
                </DataTableCell>
                <DataTableCell kind="primary">
                  <button
                    className="audit-select"
                    type="button"
                    aria-pressed={selected?.id === item.id}
                    onClick={() => {
                      setSelectedId(item.id);
                      if (window.innerWidth < 1024)
                        document.getElementById("audit-change-details")?.scrollIntoView({
                          behavior: window.matchMedia("(prefers-reduced-motion: reduce)").matches
                            ? "instant"
                            : "smooth",
                          block: "start",
                        });
                    }}
                  >
                    <bdi>{item.actorDisplayName ?? t("system")}</bdi>
                  </button>
                </DataTableCell>
                <DataTableCell label={t("columns.action")}>
                  {t(`action.${item.action}`)}
                </DataTableCell>
                <DataTableCell label={t("columns.entity")}>
                  <Badge variant="outline">{t(`entityType.${item.entityType}`)}</Badge>
                </DataTableCell>
              </DataTableRow>
            ))}
          </DataTableBody>
        </DataTable>
        {hasNextPage && (
          <Button
            type="button"
            variant="outline"
            disabled={isFetchingNextPage}
            onClick={() => {
              void fetchNextPage();
            }}
          >
            {t("loadMore")}
          </Button>
        )}
      </div>
      <aside id="audit-change-details" className="audit-inspector" aria-label={t("changesTitle")}>
        <h2>{t("changesTitle")}</h2>
        {selected && <AuditRow key={selected.id} item={selected} />}
      </aside>
    </div>
  );
}

function AuditRow({ item }: { item: AuditEntryDto }) {
  const { t } = useTranslation("audit");
  const { t: tAuth } = useTranslation("auth");
  const { t: tCommon } = useTranslation("common");
  const [copied, setCopied] = useState(false);

  const handleCopyReference = async () => {
    if (item.correlationId === null) {
      return;
    }

    try {
      await navigator.clipboard.writeText(item.correlationId);
      setCopied(true);
      setTimeout(() => {
        setCopied(false);
      }, 2000);
    } catch {
      // Clipboard access can be denied; the reference is still selectable as text.
    }
  };

  return (
    <div className="audit-event rounded-xl border border-border p-3.5">
      {item.changes.length > 0 && (
        <ul className="mt-2 space-y-1 text-sm">
          {item.changes.map((change) => (
            <li key={change.field} className="flex flex-wrap items-center gap-1.5">
              <span className="text-muted-foreground">{t(`field.${change.field}`)}:</span>
              <span dir="auto">
                {formatAuditValue(change.field, change.before, t, tAuth, tCommon)}
              </span>
              <span aria-hidden="true" className="text-muted-foreground rtl:rotate-180">
                →
              </span>
              <span dir="auto">
                {formatAuditValue(change.field, change.after, t, tAuth, tCommon)}
              </span>
            </li>
          ))}
        </ul>
      )}

      {item.correlationId !== null && (
        <div className="mt-2 flex items-center gap-1.5 text-xs text-muted-foreground">
          {/* The label stays in the surrounding (RTL-aware) direction; only the id itself is force-LTR, or the
              Arabic label would be reshuffled into the middle of the hex run by the bidi algorithm. */}
          <span>
            {t("referenceLabel")} <span dir="ltr">{item.correlationId}</span>
          </span>
          <Button
            type="button"
            variant="ghost"
            size="icon-sm"
            aria-label={t("copyReference")}
            onClick={() => {
              void handleCopyReference();
            }}
          >
            {copied ? (
              <span className="text-xs">{t("copied")}</span>
            ) : (
              <Copy className="size-3" aria-hidden="true" />
            )}
          </Button>
        </div>
      )}
    </div>
  );
}
