/* eslint-disable @typescript-eslint/only-throw-error -- TanStack Router's documented redirect idiom:
 * `redirect()` returns a Response, which the router's own data loader catches, not an Error. */
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { refreshCsrfToken } from "@/api/csrf";
import { messageFor } from "@/api/errorMessages";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import { useChangePassword } from "@/api/generated/tutoring-centre";
import { changePasswordFormSchema, type ChangePasswordFormValues } from "@/features/auth/changePasswordSchema";
import { HessaWordmark } from "@/components/brand/HessaLogo";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { useSession } from "@/features/session/useSession";
import { Button } from "@/components/ui/button";

const KnownFields = ["currentPassword", "newPassword"] as const;

export const Route = createFileRoute("/change-password")({
  beforeLoad: async ({ context }) => {
    const me = await context.queryClient.query({ ...meQueryOptions, staleTime: "static" });
    if (me === null) {
      throw redirect({ to: "/login" });
    }
  },
  component: ChangePasswordPage,
});

function ChangePasswordPage() {
  const { t } = useTranslation("auth");
  const { me } = useSession();
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const mutation = useChangePassword();

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<ChangePasswordFormValues>({ resolver: zodResolver(changePasswordFormSchema) });

  // The guard above already resolved a signed-in user before this renders.
  if (me === null) {
    return null;
  }

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        data: { currentPassword: values.currentPassword, newPassword: values.newPassword },
      });
      // The session cookie is re-issued on a successful change, so the antiforgery token (bound to the
      // signed-in user) must be refreshed before the next unsafe request — the same pattern login and
      // centre-selection already follow.
      await refreshCsrfToken();
      await queryClient.invalidateQueries({ queryKey: meQueryOptions.queryKey });
      toast.success(t("changePassword.toastSuccess"));
      await navigate({ to: me.activeCentreId === null ? "/select-centre" : "/" });
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.code === "auth.current_password_invalid") {
        setError("currentPassword", { type: "server", message: messageFor(apiError) });
        return;
      }

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
    }
  });

  return (
    <main className="flex min-h-dvh flex-col bg-muted/30">
      <header className="flex flex-wrap items-center justify-between gap-3 px-6 py-5 sm:px-10">
        <HessaWordmark />
        <LanguageSwitcher />
      </header>

      <div className="mx-auto flex w-full max-w-sm flex-1 flex-col justify-center gap-6 px-4 pb-16 sm:px-0">
        <div className="space-y-1.5">
          <h1 className="font-heading text-2xl font-semibold tracking-tight">{t("changePassword.title")}</h1>
          <p className="text-sm text-muted-foreground">
            {me.mustChangePassword ? t("changePassword.firstLoginSubtitle") : t("changePassword.subtitle")}
          </p>
        </div>

        <form
          className="space-y-4"
          onSubmit={(event) => {
            void onSubmit(event);
          }}
          noValidate
        >
          <div className="space-y-1.5">
            <label className="text-sm font-medium" htmlFor="current-password">
              {t("changePassword.currentPasswordLabel")}
            </label>
            <input
              id="current-password"
              type="password"
              autoComplete="current-password"
              disabled={isSubmitting}
              className="h-11 w-full rounded-lg border border-input bg-background px-3 text-sm outline-none transition-colors hover:border-ring/50 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive"
              aria-invalid={errors.currentPassword !== undefined}
              {...register("currentPassword")}
            />
            {errors.currentPassword !== undefined && (
              <p className="text-sm text-destructive">
                {errors.currentPassword.type === "server"
                  ? errors.currentPassword.message
                  : t(errors.currentPassword.message ?? "")}
              </p>
            )}
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium" htmlFor="new-password">
              {t("changePassword.newPasswordLabel")}
            </label>
            <input
              id="new-password"
              type="password"
              autoComplete="new-password"
              disabled={isSubmitting}
              className="h-11 w-full rounded-lg border border-input bg-background px-3 text-sm outline-none transition-colors hover:border-ring/50 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive"
              aria-invalid={errors.newPassword !== undefined}
              {...register("newPassword")}
            />
            {errors.newPassword !== undefined && (
              <p className="text-sm text-destructive">
                {errors.newPassword.type === "server" ? errors.newPassword.message : t(errors.newPassword.message ?? "")}
              </p>
            )}
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium" htmlFor="confirm-password">
              {t("changePassword.confirmPasswordLabel")}
            </label>
            <input
              id="confirm-password"
              type="password"
              autoComplete="new-password"
              disabled={isSubmitting}
              className="h-11 w-full rounded-lg border border-input bg-background px-3 text-sm outline-none transition-colors hover:border-ring/50 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive"
              aria-invalid={errors.confirmPassword !== undefined}
              {...register("confirmPassword")}
            />
            {errors.confirmPassword !== undefined && (
              <p className="text-sm text-destructive">
                {errors.confirmPassword.type === "server"
                  ? errors.confirmPassword.message
                  : t(errors.confirmPassword.message ?? "")}
              </p>
            )}
          </div>

          <Button type="submit" size="lg" className="h-11 w-full text-sm" disabled={isSubmitting}>
            {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
            {isSubmitting ? t("changePassword.submitting") : t("changePassword.submit")}
          </Button>
        </form>
      </div>
    </main>
  );
}
