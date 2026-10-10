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
      refetchMeOnPermissionDenied(apiError, queryClient);

      if (apiError.code === "concurrency.stale") {
        await queryClient.invalidateQueries({ queryKey: getGetCentreSettingsQueryKey() });
      }
    }
  });

  if (isPending) {
    return (
      <section className="space-y-4">
        <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>
        <div role="status" aria-label={t("loading.label")} className="max-w-md space-y-3">
          <Skeleton className="h-8 w-full" />
          <Skeleton className="h-8 w-full" />
          <Skeleton className="h-8 w-full" />
        </div>
      </section>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <section className="space-y-4">
        <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>
        <Alert variant="destructive">
          <AlertTitle>{t("error.title")}</AlertTitle>
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

  return (
    <section className="space-y-4">
      <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>
      <form
        className="max-w-md space-y-4"
        onSubmit={(event) => {
          void onSubmit(event);
        }}
        noValidate
      >
        <div className="space-y-1.5">
          <Label htmlFor="settings-name">{t("form.nameLabel")}</Label>
          <Input
            id="settings-name"
            dir="auto"
            disabled={isSubmitting}
            aria-invalid={errors.name !== undefined}
            {...register("name")}
          />
          {errors.name !== undefined && (
            <p className="text-sm text-destructive">
              {errors.name.type === "server" ? errors.name.message : t(errors.name.message ?? "")}
            </p>
          )}
        </div>

        <div className="space-y-1.5">
          <Label htmlFor="settings-locale">{t("form.defaultLocaleLabel")}</Label>
          <Controller
            control={control}
            name="defaultLocale"
            render={({ field }) => (
              <LocaleSelect value={field.value} onValueChange={field.onChange} disabled={isSubmitting} />
            )}
          />
        </div>

        <div className="space-y-1.5">
          <Label htmlFor="settings-slug">{t("form.slugLabel")}</Label>
          <Input id="settings-slug" dir="ltr" value={data.slug} disabled readOnly />
          <p className="text-sm text-muted-foreground">{t("form.slugHint")}</p>
        </div>

        <div className="space-y-1.5">
          <Label htmlFor="settings-timezone">{t("form.timeZoneLabel")}</Label>
          <Input id="settings-timezone" dir="ltr" value={data.timeZoneId} disabled readOnly />
          <p className="text-sm text-muted-foreground">{t("form.timeZoneHint")}</p>
        </div>

        <Button type="submit" disabled={!isDirty || isSubmitting}>
          {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
          {t("form.save")}
        </Button>
      </form>
    </section>
  );
}

function LocaleSelect({
  value,
  onValueChange,
  disabled,
}: {
  value: SupportedLocale;
  onValueChange: (value: SupportedLocale) => void;
  disabled: boolean;
}) {
  const { t } = useTranslation("common");

  return (
    <Select
      value={value}
      onValueChange={(next) => {
        if (next !== null) {
          onValueChange(next);
        }
      }}
      disabled={disabled}
    >
      <SelectTrigger id="settings-locale" className="w-full">
        <SelectValue>{(current: SupportedLocale) => (current === "ar" ? t("language.arabic") : t("language.english"))}</SelectValue>
      </SelectTrigger>
      <SelectContent>
        <SelectItem value={SupportedLocale.ar}>{t("language.arabic")}</SelectItem>
        <SelectItem value={SupportedLocale.en}>{t("language.english")}</SelectItem>
      </SelectContent>
    </Select>
  );
}
