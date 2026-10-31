import { useInfiniteQuery } from "@tanstack/react-query";
import { useTranslation } from "react-i18next";
import { apiFetch, asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import type { StudentChargesPage } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { useCan } from "@/features/session/useCan";
import { Permissions } from "@/features/session/permissions";
import { ChargesTable } from "./ChargesTable";
import { ReceiptText } from "lucide-react";
import { EmptyState } from "@/components/shared/EmptyState";
import { Skeleton } from "@/components/ui/skeleton";

export function StudentBillingTab({ studentId }: { studentId: string }) {
  const allowed = useCan(Permissions.BillingView);
  return allowed ? <BillingContent studentId={studentId} /> : null;
}
function BillingContent({ studentId }: { studentId: string }) {
  const { t } = useTranslation("billing");
  const query = useInfiniteQuery({
    queryKey: ["student-charges", studentId],
    initialPageParam: null as string | null,
    queryFn: ({ pageParam, signal }) =>
      apiFetch<StudentChargesPage>(
        `/api/students/${studentId}/charges?pageSize=25${pageParam ? `&cursor=${encodeURIComponent(pageParam)}` : ""}`,
        { signal },
      ),
    getNextPageParam: (last) => last.nextCursor ?? undefined,
  });
  if (query.isPending)
    return (
      <div role="status" aria-label={t("loading")} className="space-y-3">
        <Skeleton className="h-12 w-full" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  if (query.isError)
    return (
      <div role="alert">
        <p>{messageFor(asApiError(query.error))}</p>
        <Button
          onClick={() => {
            void query.refetch();
          }}
        >
          {t("retry")}
        </Button>
      </div>
    );
  const items = query.data.pages.flatMap((page) => page.items);
  return (
    <section aria-label={t("title")} className="space-y-4">
      {items.length === 0 ? (
        <EmptyState icon={ReceiptText} title={t("empty")} />
      ) : (
        <ChargesTable items={items} />
      )}
      {query.hasNextPage && (
        <Button
          disabled={query.isFetchingNextPage}
          onClick={() => {
            void query.fetchNextPage();
          }}
        >
          {t("loadMore")}
        </Button>
      )}
    </section>
  );
}
