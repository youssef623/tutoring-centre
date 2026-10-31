import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import {
  ArrowRight,
  Mail,
  LockKeyhole,
  ClipboardCheck,
  Loader2,
  MessageCircle,
  Users,
  Wallet,
} from "lucide-react";
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
import { HessaLoginLogo } from "@/components/brand/HessaLogo";
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
    <main className="login-page min-h-dvh">
      <header className="signin-masthead">
        <HessaLoginLogo />
        <LanguageSwitcher />
      </header>
      <img className="learning-fan" src="/brand/learning-fan.png" alt="" />
      <div className="signin-story">
        <h2>{t("panel.tagline")}</h2>
      </div>
      {/* Form column */}
      <div className="login-form-column relative flex min-h-dvh flex-col px-6 py-8 sm:px-10 lg:min-h-0 lg:px-14">
        <div className="flex flex-1 items-center justify-center py-10">
          <div className="login-form-stack w-full max-w-lg">
            <div className="login-form-box w-full space-y-6">
              <div className="space-y-1.5">
                <p className="signin-welcome">{t("login.eyebrow")}</p>
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
                className="signin-form space-y-7"
                onSubmit={(event) => {
                  void onSubmit(event);
                }}
                noValidate
              >
                <div className="space-y-1.5">
                  <label className="text-sm font-medium" htmlFor="email">
                    {t("login.emailLabel")}
                  </label>
                  <div className="signin-input">
                    <Mail aria-hidden="true" />
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
                  </div>
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
                  <div className="signin-input">
                    <LockKeyhole aria-hidden="true" />
                    <input
                      id="password"
                      type="password"
                      autoComplete="current-password"
                      disabled={isSubmitting}
                      className="h-11 w-full rounded-lg border border-input bg-background px-3 text-sm outline-none transition-colors hover:border-ring/50 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive"
                      aria-invalid={errors.password !== undefined}
                      aria-describedby={
                        errors.password === undefined ? undefined : "password-error"
                      }
                      {...register("password")}
                    />
                  </div>
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
                  <ArrowRight aria-hidden="true" className="rtl:rotate-180" />
                </Button>
              </form>
            </div>
          </div>
        </div>
      </div>

      <footer className="signin-features" aria-hidden="true">
        {PANEL_FEATURES.map(({ icon: Icon, key }) => (
          <div key={key}>
            <Icon />
            <span>{t(`panel.features.${key}.title`)}</span>
          </div>
        ))}
      </footer>
    </main>
  );
}
