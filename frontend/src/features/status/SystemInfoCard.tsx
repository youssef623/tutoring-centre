import { useTranslation } from "react-i18next";
import { messageFor } from "@/api/errorMessages";
import { asApiError } from "@/api/apiFetch";
import { useGetSystemInfo } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

/** Version and migration status from GET /api/system/info through the generated client. */
export function SystemInfoCard() {
  const { t } = useTranslation("status");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useGetSystemInfo();

  if (isPending) {
    return <Skeleton aria-label={t("systemInfo.loading")} className="h-28 w-full" />;
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("systemInfo.unavailableTitle")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError)}</p>
          {apiError.correlationId !== undefined && (
            <p>{t("systemInfo.reference", { id: apiError.correlationId })}</p>
          )}
          <Button
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

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center justify-between gap-2">
          {t("systemInfo.title")}
          {data.databaseUpToDate ? (
            <Badge>{t("systemInfo.schemaUpToDate")}</Badge>
          ) : (
            <Badge variant="destructive">{t("systemInfo.migrationsPending")}</Badge>
          )}
        </CardTitle>
      </CardHeader>
      <CardContent>
        <dl className="grid grid-cols-[auto_1fr] gap-x-4 gap-y-1 text-sm">
          <dt className="text-muted-foreground">{t("systemInfo.applicationVersion")}</dt>
          <dd dir="ltr" className="text-start">
            {data.applicationVersion}
          </dd>
          <dt className="text-muted-foreground">{t("systemInfo.latestMigration")}</dt>
          <dd dir="ltr" className="text-start">
            {data.latestMigration ?? t("systemInfo.none")}
          </dd>
        </dl>
      </CardContent>
    </Card>
  );
}
