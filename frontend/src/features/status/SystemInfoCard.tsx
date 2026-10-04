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
  const { data, error, isPending, refetch } = useGetSystemInfo();

  if (isPending) {
    return <Skeleton aria-label="Loading system information" className="h-28 w-full" />;
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>System information is unavailable</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError, "en")}</p>
          {apiError.correlationId !== undefined && <p>Reference: {apiError.correlationId}</p>}
          <Button
            variant="outline"
            size="sm"
            className="self-start"
            onClick={() => {
              void refetch();
            }}
          >
            Retry
          </Button>
        </AlertDescription>
      </Alert>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center justify-between gap-2">
          System information
          {data.databaseUpToDate ? (
            <Badge>Database schema up to date</Badge>
          ) : (
            <Badge variant="destructive">Migrations pending</Badge>
          )}
        </CardTitle>
      </CardHeader>
      <CardContent>
        <dl className="grid grid-cols-[auto_1fr] gap-x-4 gap-y-1 text-sm">
          <dt className="text-muted-foreground">Application version</dt>
          <dd>{data.applicationVersion}</dd>
          <dt className="text-muted-foreground">Latest migration</dt>
          <dd>{data.latestMigration ?? "none"}</dd>
        </dl>
      </CardContent>
    </Card>
  );
}
