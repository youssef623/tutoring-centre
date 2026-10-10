/* eslint-disable @typescript-eslint/only-throw-error -- TanStack Router's documented redirect idiom:
 * `redirect()` returns a Response, which the router's own data loader catches, not an Error. */
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import {
  Building2,
  Check,
  ChevronRight,
  ClipboardCheck,
  Copy,
  GraduationCap,
  Loader2,
  LogOut,
  RefreshCw,
  Shield,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { cn } from "cn";
import { asApiError } from "@/api/apiFetch";
import { refreshCsrfToken } from "@/api/csrf";
import { showApiError } from "@/api/showApiError";
import { useSelectCentre, type MembershipDto } from "@/api/generated/tutoring-centre";
import { HessaWordmark } from "@/components/brand/HessaLogo";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { useSession } from "@/features/session/useSession";
import { useSignOut } from "@/features/session/useSignOut";
import { initialsOf } from "@/lib/initials";
import { Button } from "@/components/ui/button";

export const Route = createFileRoute("/select-centre")({
  beforeLoad: async ({ context }) => {
    const me = await context.queryClient.query({ ...meQueryOptions, staleTime: "static" });
    if (me === null) {
      throw redirect({ to: "/login" });
    }

    if (me.mustChangePassword) {
      throw redirect({ to: "/change-password" });
    }
  },
  component: SelectCentrePage,
});

/** Role badge styling, matched to the role's existing translation key (`auth:roles.*`). Decorative color beyond the badge's own text/icon — never the only signal of which role this is. */
const ROLE_STYLE: Record<string, { icon: LucideIcon; className: string }> = {
  owner: { icon: Shield, className: "bg-accent text-accent-foreground" },
  teacher: { icon: GraduationCap, className: "bg-warning/15 text-warning-foreground dark:bg-warning/25" },
  secretary: { icon: ClipboardCheck, className: "bg-blue-50 text-blue-700 dark:bg-blue-500/15 dark:text-blue-300" },
};
const DefaultRoleStyle: { icon: LucideIcon; className: string } = { icon: Shield, className: "bg-muted text-muted-foreground" };

/** A small fixed palette for the per-centre avatar tile. Picked deterministically from the centre id, purely decorative — not tied to role. */
const AVATAR_PALETTE = [
  "bg-accent text-accent-foreground",
  "bg-blue-50 text-blue-700 dark:bg-blue-500/15 dark:text-blue-300",
  "bg-warning/15 text-warning-foreground dark:bg-warning/25",
  "bg-purple-50 text-purple-700 dark:bg-purple-500/15 dark:text-purple-300",
  "bg-rose-50 text-rose-700 dark:bg-rose-500/15 dark:text-rose-300",
] as const;

function avatarClassName(centreId: string): string {
  let hash = 0;
  for (let i = 0; i < centreId.length; i += 1) {
    hash = (hash * 31 + centreId.charCodeAt(i)) >>> 0;
  }
  // hash % length is always a valid index into the non-empty palette above; the fallback only satisfies the type checker.
  return AVATAR_PALETTE[hash % AVATAR_PALETTE.length] ?? AVATAR_PALETTE[0];
}

function SelectCentrePage() {
  const { t } = useTranslation("auth");
  const { me } = useSession();
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const selectCentreMutation = useSelectCentre();
  const signOut = useSignOut();

  // The guard above already redirected a signed-out visitor; this only covers the render before that settles.
  if (me === null) {
    return null;
  }

  const isEmpty = me.memberships.length === 0;

  const handleSelect = async (centreId: string) => {
    try {
      await selectCentreMutation.mutateAsync({ data: { centreId } });
      await refreshCsrfToken();
      await queryClient.invalidateQueries({ queryKey: meQueryOptions.queryKey });
      await navigate({ to: "/" });
    } catch (error) {
      showApiError(asApiError(error));
    }
  };

  return (
    <main className="flex min-h-dvh flex-col bg-muted/30">
      <header className="flex flex-wrap items-center justify-between gap-3 px-6 py-5 sm:px-10">
        <HessaWordmark />
        <div className="flex items-center gap-2">
          <LanguageSwitcher />
          {!isEmpty && (
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => {
                void signOut();
              }}
            >
              <LogOut className="rtl:rotate-180" aria-hidden="true" />
              {t("selectCentre.logout")}
            </Button>
          )}
        </div>
      </header>

      <div className="mx-auto flex w-full max-w-xl flex-1 flex-col gap-6 px-4 pb-16 pt-4 sm:px-0">
        {isEmpty ? (
          <NoActiveCentre email={me.email} onSignOut={signOut} />
        ) : (
          <>
            <div className="space-y-1.5">
              <p className="text-sm font-medium text-primary">
                {t("selectCentre.signedInAs")} <span dir="ltr">{me.email}</span>
              </p>
              <h1 className="font-heading text-3xl font-semibold tracking-tight">{t("selectCentre.title")}</h1>
              <p className="text-sm text-muted-foreground">{t("selectCentre.subtitle", { count: me.memberships.length })}</p>
            </div>

            <div className="flex flex-col gap-2.5">
              {me.memberships.map((membership) => (
                <CentreCard key={membership.centreId} membership={membership} onSelect={handleSelect} />
              ))}
            </div>

            <p className="text-sm text-muted-foreground">
              {t("selectCentre.missingCentre")} <span dir="ltr">{me.email}</span>
            </p>
          </>
        )}
      </div>
    </main>
  );
}

function NoActiveCentre({ email, onSignOut }: { email: string; onSignOut: () => Promise<void> }) {
  const { t } = useTranslation("auth");
  const queryClient = useQueryClient();
  const [isChecking, setIsChecking] = useState(false);
  const [copied, setCopied] = useState(false);

  const handleCheckAgain = async () => {
    setIsChecking(true);
    try {
      await queryClient.refetchQueries({ queryKey: meQueryOptions.queryKey });
    } finally {
      setIsChecking(false);
    }
  };

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(email);
      setCopied(true);
      setTimeout(() => {
        setCopied(false);
      }, 2000);
    } catch {
      // Clipboard access can be denied (permissions, insecure context); the email is still selectable as text.
    }
  };

  return (
    <div className="flex flex-1 flex-col items-center gap-5 pt-12 text-center">
      <span
        aria-hidden="true"
        className="flex size-18 shrink-0 items-center justify-center rounded-2xl bg-accent text-accent-foreground"
      >
        <Building2 className="size-8.5" strokeWidth={1.6} />
      </span>

      <div className="space-y-2">
        <h1 className="font-heading text-2xl font-semibold tracking-tight">{t("selectCentre.noActiveCentre.title")}</h1>
        <p className="max-w-sm text-base leading-relaxed text-muted-foreground">
          {t("selectCentre.noActiveCentre.description")}
        </p>
      </div>

      <div className="flex w-full flex-col gap-1.5 rounded-2xl border border-border bg-card p-4 text-start shadow-sm">
        <p className="text-xs font-semibold text-muted-foreground">{t("selectCentre.noActiveCentre.inviteLabel")}</p>
        <div className="flex flex-wrap items-center gap-3">
          <span dir="ltr" className="flex-1 text-base font-semibold">
            {email}
          </span>
          <Button type="button" variant="outline" size="sm" onClick={() => void handleCopy()}>
            {copied ? <Check aria-hidden="true" /> : <Copy aria-hidden="true" />}
            {copied ? t("selectCentre.noActiveCentre.copied") : t("selectCentre.noActiveCentre.copy")}
          </Button>
        </div>
      </div>

      <div className="flex flex-wrap justify-center gap-2.5">
        <Button type="button" onClick={() => void handleCheckAgain()} disabled={isChecking}>
          {isChecking ? (
            <Loader2 className="animate-spin" aria-hidden="true" />
          ) : (
            <RefreshCw aria-hidden="true" />
          )}
          {isChecking ? t("selectCentre.noActiveCentre.checking") : t("selectCentre.noActiveCentre.checkAgain")}
        </Button>
        <Button type="button" variant="outline" onClick={() => void onSignOut()}>
          {t("selectCentre.logout")}
        </Button>
      </div>
    </div>
  );
}

function CentreCard({
  membership,
  onSelect,
}: {
  membership: MembershipDto;
  onSelect: (centreId: string) => void | Promise<void>;
}) {
  const { t } = useTranslation("auth");
  const role = ROLE_STYLE[String(membership.role)] ?? DefaultRoleStyle;
  const RoleIcon = role.icon;

  return (
    <button
      type="button"
      className="flex w-full items-center gap-3.5 rounded-2xl border border-border bg-card p-3.5 text-start text-card-foreground shadow-sm outline-none transition-colors hover:border-ring/50 hover:bg-muted focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"
      onClick={() => {
        void onSelect(membership.centreId);
      }}
    >
      <span
        aria-hidden="true"
        className={cn(
          "flex size-12 shrink-0 items-center justify-center rounded-xl text-base font-bold",
          avatarClassName(membership.centreId),
        )}
      >
        {initialsOf(membership.centreName)}
      </span>
      <span className="min-w-0 flex-1 space-y-0.5">
        <span className="block text-base font-bold" dir="auto">
          {membership.centreName}
        </span>
      </span>
      <span
        className={cn(
          "flex shrink-0 items-center gap-1.5 whitespace-nowrap rounded-full px-2.5 py-1 text-xs font-semibold",
          role.className,
        )}
      >
        <RoleIcon className="size-3.5" aria-hidden="true" />
        {t(`roles.${String(membership.role)}`)}
      </span>
      <ChevronRight className="size-4.5 shrink-0 text-muted-foreground rtl:rotate-180" aria-hidden="true" />
    </button>
  );
}
