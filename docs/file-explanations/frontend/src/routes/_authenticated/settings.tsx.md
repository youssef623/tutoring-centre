# `frontend/src/routes/_authenticated/settings.tsx`

## Purpose

Declares `SettingsPage`, `SettingsPageContent`, `LocaleSelect`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/settings.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 230 lines and 7,837 bytes. Source hash: `f0d6cbffc0d3837a98e597f3f615b11a8da8db34a85df05869cd801a6f70250d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { createFileRoute } from "@tanstack/react-router";`
- Line 4: `import { Loader2 } from "lucide-react";`
- Line 5: `import { Controller, useForm } from "react-hook-form";`
- Line 6: `import { useTranslation } from "react-i18next";`
- Line 7: `import { toast } from "sonner";`
- Line 8: `import { asApiError } from "@/api/apiFetch";`
- Line 9: `import { messageFor } from "@/api/errorMessages";`
- Line 10: `import { applyFieldErrors, showApiError } from "@/api/showApiError";`
- Line 11: `import {`
- Line 18: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 19: `import { Button } from "@/components/ui/button";`
- Line 20: `import { Input } from "@/components/ui/input";`
- Line 21: `import { Label } from "@/components/ui/label";`
- Line 22: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`
- Line 23: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 24: `import { centreSettingsFormSchema, type CentreSettingsFormValues } from "@/features/settings/settingsSchema";`
- Line 25: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 26: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 27: `import { Permissions } from "@/features/session/permissions";`
- Line 28: `import { PageHeader } from "@/components/shared/PageHeader";`
- Line 29: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 31](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L31): `export const Route = createFileRoute("/_authenticated/settings")({`
- [Line 37](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L37): `function SettingsPage() {`
- [Line 45](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L45): `function SettingsPageContent() {`
- [Line 200](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L200): `function LocaleSelect({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/settings.test.tsx:42](../../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L42)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getGetCentreSettingsQueryKey,
  getGetMeQueryKey,
  useGetCentreSettings,
  useUpdateCentreSettings,
  SupportedLocale,
} from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { centreSettingsFormSchema, type CentreSettingsFormValues } from "@/features/settings/settingsSchema";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";
import { PageHeader } from "@/components/shared/PageHeader";
import { isolateLtr } from "@/lib/bidi";

export const Route = createFileRoute("/_authenticated/settings")({
  component: SettingsPage,
});

const KnownFields = ["name", "defaultLocale"] as const;

function SettingsPage() {
  return (
    <RouteGuard permission={Permissions.CentreSettingsManage}>
      <SettingsPageContent />
    </RouteGuard>
  );
}

function SettingsPageContent() {
  const { t } = useTranslation("settings");
  const { t: tCommon } = useTranslation("common");
  const queryClient = useQueryClient();
  const { data, error, isPending, refetch } = useGetCentreSettings();
  const mutation = useUpdateCentreSettings();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<CentreSettingsFormValues>({
    resolver: zodResolver(centreSettingsFormSchema),
    // Synced from the query's data every time it changes identity (a fresh fetch or a post-save refetch) —
    // React Hook Form's own mechanism for a form backed by query data, no effect needed.
    values: data === undefined ? undefined : { name: data.name, defaultLocale: data.defaultLocale },
  });

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetCentreSettingsQueryKey() }),
      queryClient.invalidateQueries({ queryKey: getGetMeQueryKey() }),
    ]);

  const onSubmit = handleSubmit(async (values) => {
    if (data === undefined) {
      return;
    }

    try {
      await mutation.mutateAsync({
        data: { name: values.name, defaultLocale: values.defaultLocale, version: data.version },
      });
      await invalidate();
      toast.success(t("toasts.saved"));
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
```

The excerpt stops at line 90; use the original file for the remaining 140 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
