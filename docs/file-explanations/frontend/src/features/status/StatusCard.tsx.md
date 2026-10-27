# `frontend/src/features/status/StatusCard.tsx`

## Purpose

Declares `StatusCard`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/status/StatusCard.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 68 lines and 2,353 bytes. Source hash: `a6b3b60178a8ef22e2ac382cf143cd2c5c3d5463f6cc814fd1e7903dafb3d2f2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 3: `import { Button } from "@/components/ui/button";`
- Line 4: `import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";`
- Line 5: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 6: `import { useReadiness } from "./useReadiness";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/features/status/StatusCard.tsx#L8): `export function StatusCard() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/status/StatusCard.test.tsx:6](../../../../../../frontend/src/features/status/StatusCard.test.tsx#L6)
- [frontend/src/routes/status.tsx:3](../../../../../../frontend/src/routes/status.tsx#L3)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
