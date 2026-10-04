import { useTranslation } from "react-i18next";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { useReadiness } from "./useReadiness";

export function StatusCard() {
  const { t } = useTranslation("status");
  const { t: tCommon } = useTranslation("common");
  const readiness = useReadiness();

  const retry = () => {
    void readiness.refetch();
  };

  if (readiness.isPending) {
    return (
      <div role="status" aria-label={t("readiness.checking")} className="w-full max-w-md space-y-3">
        <Skeleton className="h-6 w-2/3" />
        <Skeleton className="h-4 w-full" />
      </div>
    );
  }

  if (readiness.isError) {
    return (
      <Alert variant="destructive" className="w-full max-w-md">
        <AlertTitle>{t("readiness.unreachableTitle")}</AlertTitle>
        <AlertDescription>
          <p>{t("readiness.unreachableBody")}</p>
          <Button variant="outline" size="sm" className="mt-3" onClick={retry}>
            {tCommon("retry")}
          </Button>
        </AlertDescription>
      </Alert>
    );
  }

  if (readiness.data.state === "unavailable") {
    return (
      <Alert className="w-full max-w-md border-amber-500/50 text-amber-900 dark:text-amber-200">
        <AlertTitle>{t("readiness.databaseUnavailableTitle")}</AlertTitle>
        <AlertDescription>
          <p>{t("readiness.databaseUnavailableBody")}</p>
          <Button variant="outline" size="sm" className="mt-3" onClick={retry}>
            {tCommon("retry")}
          </Button>
        </AlertDescription>
      </Alert>
    );
  }

  return (
    <Card className="w-full max-w-md">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <span aria-hidden="true" className="size-3 rounded-full bg-green-500" />
          {t("readiness.healthyTitle")}
        </CardTitle>
        <CardDescription>{t("readiness.healthyDescription")}</CardDescription>
      </CardHeader>
      <CardContent className="text-sm text-muted-foreground">
        {t("readiness.lastCheckedAt", { time: readiness.data.checkedAt.toLocaleTimeString() })}
      </CardContent>
    </Card>
  );
}
