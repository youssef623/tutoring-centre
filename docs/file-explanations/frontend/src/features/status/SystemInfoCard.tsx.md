# `frontend/src/features/status/SystemInfoCard.tsx`

## Purpose

Source comment (intent): Version and migration status from GET /api/system/info through the generated client.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/status/SystemInfoCard.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 74 lines and 2,633 bytes. Source hash: `9173cf534ca32cf6d01e7228491f45177f756002a1a4562898acc811a3b121bc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { messageFor } from "@/api/errorMessages";`
- Line 3: `import { asApiError } from "@/api/apiFetch";`
- Line 4: `import { useGetSystemInfo } from "@/api/generated/tutoring-centre";`
- Line 5: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 6: `import { Badge } from "@/components/ui/badge";`
- Line 7: `import { Button } from "@/components/ui/button";`
- Line 8: `import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";`
- Line 9: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 10: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../frontend/src/features/status/SystemInfoCard.tsx#L13): `export function SystemInfoCard() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/status/SystemInfoCard.test.tsx:6](../../../../../../frontend/src/features/status/SystemInfoCard.test.tsx#L6)
- [frontend/src/routes/status.tsx:4](../../../../../../frontend/src/routes/status.tsx#L4)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useTranslation } from "react-i18next";
import { messageFor } from "@/api/errorMessages";
import { asApiError } from "@/api/apiFetch";
import { useGetSystemInfo } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { isolateLtr } from "@/lib/bidi";

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
            <p>{t("systemInfo.reference", { id: isolateLtr(apiError.correlationId) })}</p>
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
