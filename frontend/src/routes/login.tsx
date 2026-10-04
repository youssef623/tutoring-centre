import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { z } from "zod";
import { applyFieldErrors } from "@/api/showApiError";
import { messageFor } from "@/api/errorMessages";
import { refreshCsrfToken } from "@/api/csrf";
import { asApiError } from "@/api/apiFetch";
import { useLogin, type MeDto } from "@/api/generated/tutoring-centre";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

export interface LoginSearch {
  redirect?: string;
}

export const Route = createFileRoute("/login")({
  validateSearch: (search: Record<string, unknown>): LoginSearch => ({
    redirect: typeof search.redirect === "string" ? search.redirect : undefined,
  }),
  component: LoginPage,
});

const loginSchema = z.object({
  email: z.string().min(1, "validation.emailRequired").pipe(z.email("validation.emailInvalid")),
  password: z.string().min(1, "validation.passwordRequired"),
});

type LoginFormValues = z.infer<typeof loginSchema>;
const KnownFields = ["email", "password"] as const;

/** Only ever a relative, same-site path: an open `redirect` search parameter is never trusted as-is. */
function resolveTarget(me: MeDto, redirect: string | undefined): string {
  if (me.activeCentreId === null) {
    return "/select-centre";
  }

  if (redirect !== undefined && redirect.startsWith("/") && !redirect.startsWith("//")) {
    return redirect;
  }

  return "/";
}

function LoginPage() {
  const { t } = useTranslation("auth");
  const { redirect } = Route.useSearch();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const loginMutation = useLogin();
  const [banner, setBanner] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({ resolver: zodResolver(loginSchema) });

  const onSubmit = handleSubmit(async (values) => {
    setBanner(null);
    try {
      const me = await loginMutation.mutateAsync({ data: values });
      await refreshCsrfToken();
      queryClient.setQueryData(meQueryOptions.queryKey, me);
      await navigate({ href: resolveTarget(me, redirect) });
    } catch (error) {
      const apiError = asApiError(error);
      if (apiError.code === "auth.invalid_credentials") {
        setBanner(t("errors.invalidCredentials"));
      } else if (apiError.code === "auth.rate_limited") {
        setBanner(t("errors.rateLimited"));
      } else {
        applyFieldErrors(apiError, setError, KnownFields);
        if (apiError.fieldErrors === undefined) {
          setBanner(messageFor(apiError));
        }
      }
    }
  });

  return (
    <main className="flex min-h-screen items-center justify-center bg-muted/40 p-6">
      <div className="w-full max-w-sm space-y-4">
        <div className="flex items-center justify-between">
          <span className="text-xs font-semibold tracking-wide text-muted-foreground uppercase">
            {t("login.brand")}
          </span>
          <LanguageSwitcher />
        </div>
        <Card className="shadow-sm">
          <CardContent className="space-y-5">
            <div className="space-y-1 text-center">
              <h1 className="text-2xl font-semibold tracking-tight">{t("login.title")}</h1>
              <p className="text-sm text-muted-foreground">{t("login.subtitle")}</p>
            </div>
            {banner !== null && (
              <Alert variant="destructive">
                <AlertDescription>{banner}</AlertDescription>
              </Alert>
            )}
            <form
              className="space-y-4"
              onSubmit={(event) => {
                void onSubmit(event);
              }}
              noValidate
            >
              <div className="space-y-1.5">
                <label className="text-sm font-medium" htmlFor="email">
                  {t("login.emailLabel")}
                </label>
                <input
                  id="email"
                  type="email"
                  autoComplete="email"
                  disabled={isSubmitting}
                  className="h-10 w-full rounded-lg border border-input bg-background px-3 text-sm outline-none transition-colors hover:border-ring/50 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive"
                  aria-invalid={errors.email !== undefined}
                  {...register("email")}
                />
                {errors.email !== undefined && (
                  <p className="text-sm text-destructive">
                    {errors.email.type === "server" ? errors.email.message : t(errors.email.message ?? "")}
                  </p>
                )}
              </div>
              <div className="space-y-1.5">
                <label className="text-sm font-medium" htmlFor="password">
                  {t("login.passwordLabel")}
                </label>
                <input
                  id="password"
                  type="password"
                  autoComplete="current-password"
                  disabled={isSubmitting}
                  className="h-10 w-full rounded-lg border border-input bg-background px-3 text-sm outline-none transition-colors hover:border-ring/50 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive"
                  aria-invalid={errors.password !== undefined}
                  {...register("password")}
                />
                {errors.password !== undefined && (
                  <p className="text-sm text-destructive">
                    {errors.password.type === "server" ? errors.password.message : t(errors.password.message ?? "")}
                  </p>
                )}
              </div>
              <Button type="submit" className="w-full" disabled={isSubmitting}>
                {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
                {isSubmitting ? t("login.submitting") : t("login.submit")}
              </Button>
            </form>
          </CardContent>
        </Card>
      </div>
    </main>
  );
}
