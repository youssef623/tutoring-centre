import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { ClipboardCheck, Loader2, MessageCircle, Users, Wallet } from "lucide-react";
import type { LucideIcon } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { z } from "zod";
import { applyFieldErrors } from "@/api/showApiError";
import { messageFor } from "@/api/errorMessages";
import { refreshCsrfToken } from "@/api/csrf";
import { asApiError } from "@/api/apiFetch";
import { login, type MeDto } from "@/api/generated/tutoring-centre";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { isPortalRole, sessionHome } from "@/features/session/roleShell";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { HessaMark, HessaWordmark } from "@/components/brand/HessaLogo";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import i18next from "@/i18n";

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

const PANEL_FEATURES: { icon: LucideIcon; key: "attendance" | "groups" | "fees" | "assistant" }[] =
  [
    { icon: ClipboardCheck, key: "attendance" },
    { icon: Users, key: "groups" },
    { icon: Wallet, key: "fees" },
    { icon: MessageCircle, key: "assistant" },
  ];

/** Only ever a relative, same-site path: an open `redirect` search parameter is never trusted as-is. */
function resolveTarget(me: MeDto, redirect: string | undefined): string {
  if (me.mustChangePassword || isPortalRole(me.activeRole)) return sessionHome(me);
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
      const me = await login(values);
      if (
        isPortalRole(me.activeRole) ||
        me.memberships.some((membership) => isPortalRole(membership.role))
      ) {
        await i18next.changeLanguage(me.preferredLocale === "ar" ? "ar" : "en");
      }
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
    <main className="login-page min-h-dvh lg:grid lg:grid-cols-2">
      {/* Form column */}
      <div className="login-form-column relative flex min-h-dvh flex-col px-6 py-8 sm:px-10 lg:min-h-0 lg:px-14">
        <div className="login-topbar flex items-center justify-end">
          <LanguageSwitcher />
        </div>

        <div className="flex flex-1 items-center justify-center py-10">
          <div className="login-form-stack w-full max-w-lg">
            <HessaWordmark className="login-wordmark" />
          <div className="login-form-box w-full space-y-6">
            <div className="space-y-1.5">
              <p className="text-sm font-medium text-primary">{t("login.eyebrow")}</p>
              <h1 className="font-heading text-3xl font-semibold tracking-tight">
                {t("login.title")}
              </h1>
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
                  className="h-11 w-full rounded-lg border border-input bg-background px-3 text-sm outline-none transition-colors hover:border-ring/50 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive"
                  aria-invalid={errors.email !== undefined}
                  aria-describedby={errors.email === undefined ? undefined : "email-error"}
                  {...register("email")}
                />
                {errors.email !== undefined && (
                  <p id="email-error" className="text-sm text-destructive">
                    {errors.email.type === "server"
                      ? errors.email.message
                      : t(errors.email.message ?? "")}
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
                  className="h-11 w-full rounded-lg border border-input bg-background px-3 text-sm outline-none transition-colors hover:border-ring/50 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive"
                  aria-invalid={errors.password !== undefined}
                  aria-describedby={errors.password === undefined ? undefined : "password-error"}
                  {...register("password")}
                />
                {errors.password !== undefined && (
                  <p id="password-error" className="text-sm text-destructive">
                    {errors.password.type === "server"
                      ? errors.password.message
                      : t(errors.password.message ?? "")}
                  </p>
                )}
              </div>
              <Button
                type="submit"
                size="lg"
                className="h-11 w-full text-sm"
                disabled={isSubmitting}
              >
                {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
                {isSubmitting ? t("login.submitting") : t("login.submit")}
              </Button>
            </form>
          </div>
          </div>
        </div>
      </div>

      {/* Brand panel */}
      <aside
        className="login-panel relative hidden overflow-hidden text-white lg:flex lg:flex-col lg:justify-center"
        aria-hidden="true"
      >
        {/* decorative glows. Tailwind arbitrary-value utilities, not a style attribute: the CSS lands in the
            build's stylesheet rather than as an inline style, which a strict CSP (style-src 'self') blocks. */}

        <div className="login-panel-content relative z-10 mx-auto flex w-full flex-col gap-8 px-10 xl:px-20">
          <div className="flex items-center gap-5">
            <HessaMark className="size-24" />
            <span className="flex flex-col gap-2 leading-none">
              <span className="font-heading text-4xl font-semibold tracking-tight">Hessa</span>
              <span dir="rtl" className="text-3xl font-medium text-white/80">
                حصة
              </span>
            </span>
          </div>

          <div className="space-y-0">
            <h2 className="font-heading text-4xl leading-tight font-semibold tracking-tight text-balance xl:text-5xl">
              {t("panel.tagline")}
            </h2>
            <p className="mt-4 text-lg text-white/80">{t("panel.description")}</p>
          </div>

          <ul className="space-y-3">
            {PANEL_FEATURES.map(({ icon: Icon, key }) => (
              <li key={key} className="login-feature flex items-start gap-4 py-4">
                <span className="flex size-10 shrink-0 items-center justify-center text-[#b397ff]">
                  <Icon className="size-7" strokeWidth={1.75} />
                </span>
                <div className="space-y-0.5">
                  <p className="text-base font-semibold">{t(`panel.features.${key}.title`)}</p>
                  <p className="text-base text-white/75">{t(`panel.features.${key}.description`)}</p>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </aside>
    </main>
  );
}
