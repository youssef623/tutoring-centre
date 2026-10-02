import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { useReadiness } from "./useReadiness";

export function StatusCard() {
  const readiness = useReadiness();

  const retry = () => {
    void readiness.refetch();
  };

  if (readiness.isPending) {
    return (
      <div role="status" aria-label="Checking API status" className="w-full max-w-md space-y-3">
        <Skeleton className="h-6 w-2/3" />
        <Skeleton className="h-4 w-full" />
      </div>
    );
  }

  if (readiness.isError) {
    return (
      <Alert variant="destructive" className="w-full max-w-md">
        <AlertTitle>Cannot reach the API</AlertTitle>
        <AlertDescription>
          <p>The API did not respond. Check that it is running, then try again.</p>
          <Button variant="outline" size="sm" className="mt-3" onClick={retry}>
            Retry
          </Button>
        </AlertDescription>
      </Alert>
    );
  }

  if (readiness.data.state === "unavailable") {
    return (
      <Alert className="w-full max-w-md border-amber-500/50 text-amber-900 dark:text-amber-200">
        <AlertTitle>API is running but the database is unavailable</AlertTitle>
        <AlertDescription>
          <p>Start PostgreSQL, then try again.</p>
          <Button variant="outline" size="sm" className="mt-3" onClick={retry}>
            Retry
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
          Healthy
        </CardTitle>
        <CardDescription>API and database are reachable</CardDescription>
      </CardHeader>
      <CardContent className="text-sm text-muted-foreground">
        Last checked at {readiness.data.checkedAt.toLocaleTimeString()}
      </CardContent>
    </Card>
  );
}
