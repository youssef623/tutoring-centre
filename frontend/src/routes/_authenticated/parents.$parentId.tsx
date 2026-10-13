import { Link, createFileRoute } from "@tanstack/react-router";
import { SearchX } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetParent, type ParentChildDto, type ParentDetailDto } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { PhoneText } from "@/components/shared/PhoneText";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Skeleton } from "@/components/ui/skeleton";
import { ParentEditDialog } from "@/features/parents/ParentEditDialog";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";
import { useCan } from "@/features/session/useCan";

export const Route = createFileRoute("/_authenticated/parents/$parentId")({
  component: ParentDetailPage,
});

function ParentDetailPage() {
  return (
    <RouteGuard permission={Permissions.StudentsView}>
      <ParentDetailPageContent />
    </RouteGuard>
  );
}

function ParentDetailPageContent() {
  const { parentId } = Route.useParams();
  const { t } = useTranslation("parents");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useGetParent(parentId);

  if (isPending) {
    return (
      <section className="space-y-4">
        <div role="status" aria-label={t("loading.detailLabel")} className="max-w-md space-y-3">
          <Skeleton className="h-8 w-48" />
          <Skeleton className="h-24 w-full" />
        </div>
      </section>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    if (apiError.kind === "notFound") {
      return <ParentNotFound />;
    }

    return (
      <section className="space-y-4">
        <Alert variant="destructive">
          <AlertTitle>{t("detailError.title")}</AlertTitle>
          <AlertDescription className="flex flex-col gap-2">
            <p>{messageFor(apiError)}</p>
            {apiError.correlationId !== undefined && (
              <p>{t("error.reference", { correlationId: apiError.correlationId })}</p>
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

  return <ParentDetailContent parent={data} />;
}

/** Also what a foreign parent id (another centre's) renders: the read service never reveals that it exists. */
function ParentNotFound() {
  const { t } = useTranslation("parents");

  return (
    <div className="flex flex-col items-center gap-3 py-16 text-center">
      <SearchX className="size-10 text-muted-foreground" aria-hidden="true" />
      <h1 className="text-xl font-semibold tracking-tight">{t("notFound.title")}</h1>
      <p className="max-w-sm text-sm text-muted-foreground">{t("notFound.description")}</p>
      <Button render={<Link to="/students" />}>{t("notFound.backToStudents")}</Button>
    </div>
  );
}

function ParentDetailContent({ parent }: { parent: ParentDetailDto }) {
  const { t } = useTranslation("parents");
  const canManage = useCan(Permissions.StudentsManage);
  const [editOpen, setEditOpen] = useState(false);

  return (
    <section className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight" dir="auto">
          {parent.fullName}
        </h1>
        {canManage && (
          <Button
            type="button"
            variant="outline"
            onClick={() => {
              setEditOpen(true);
            }}
          >
            {t("actions.edit")}
          </Button>
        )}
      </div>

      <div className="max-w-md space-y-3 rounded-xl border border-border p-4">
        <div className="flex items-center justify-between gap-4">
          <span className="text-sm text-muted-foreground">{t("card.phoneLabel")}</span>
          <PhoneText value={parent.phone} />
        </div>
      </div>

      <div className="max-w-md space-y-3">
        <h2 className="text-lg font-semibold tracking-tight">{t("children.title")}</h2>

        {parent.children.length === 0 ? (
          <div className="rounded-xl border border-dashed border-border py-10 text-center">
            <p className="text-sm font-medium">{t("children.emptyState")}</p>
          </div>
        ) : (
          <div className="overflow-x-auto rounded-xl border border-border">
            <table className="w-full min-w-[420px] text-start text-sm">
              <thead className="bg-muted/50 text-muted-foreground">
                <tr>
                  <th className="px-4 py-2 text-start font-medium">{t("children.columns.name")}</th>
                  <th className="px-4 py-2 text-start font-medium">{t("children.columns.relationship")}</th>
                  <th className="px-4 py-2 text-start font-medium">{t("children.columns.status")}</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {parent.children.map((child) => (
                  <ChildRow key={child.studentId} child={child} />
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {canManage && <ParentEditDialog parent={parent} open={editOpen} onOpenChange={setEditOpen} />}
    </section>
  );
}

function ChildRow({ child }: { child: ParentChildDto }) {
  const { t } = useTranslation("parents");

  return (
    <tr>
      <td className="px-4 py-2" dir="auto">
        <Link to="/students/$studentId" params={{ studentId: child.studentId }} className="font-medium hover:underline">
          {child.fullName}
        </Link>
      </td>
      <td className="px-4 py-2">
        <div className="flex items-center gap-2" dir="auto">
          {t(`relationship.${child.relationship}`)}
          {child.isPrimary && <Badge variant="outline">{t("children.primaryBadge")}</Badge>}
        </div>
      </td>
      <td className="px-4 py-2">
        <StatusBadge status={child.status} />
      </td>
    </tr>
  );
}
