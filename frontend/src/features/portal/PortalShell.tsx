import { ArrowRight, CalendarDays, MessageCircle } from "lucide-react";
import { Link } from "@tanstack/react-router";
import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import type { MeDto } from "@/api/generated/tutoring-centre";
import { HessaWordmark } from "@/components/brand/HessaLogo";
import { buttonVariants } from "@/components/ui/button-variants";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { UserMenu } from "@/features/shell/AppShell";

export function PortalShell({ me, children }: { me: MeDto; children?: ReactNode }) {
  const { t } = useTranslation("portal");
  const centre = me.memberships.find((item) => item.centreId === me.activeCentreId);
  const name = me.portal?.subjectName ?? me.displayName;
  return (
    <div className="portal-shell min-h-dvh w-full bg-background">
      <div className="flex min-h-dvh w-full flex-col overflow-hidden">
        <header className="portal-header flex flex-wrap items-center justify-between gap-5 border-b border-primary/15 bg-card px-5 py-5 sm:px-8">
          <div className="space-y-1">
            <HessaWordmark />
            <p className="text-sm text-muted-foreground" dir="auto">
              {centre?.centreName}
            </p>
          </div>
          <nav
            aria-label={t("navigation")}
            className="order-3 flex w-full justify-center gap-6 border-t border-border pt-4 md:order-0 md:w-auto md:border-0 md:pt-0"
          >
            <Link
              to="/portal/chat"
              className="flex items-center gap-2 px-3 py-2 text-sm font-medium"
              activeOptions={{ exact: true, includeSearch: false }}
              activeProps={{ className: "border-b-2 border-primary text-primary" }}
              inactiveProps={{ className: "border-b-2 border-transparent text-muted-foreground" }}
            >
              <MessageCircle className="size-4" aria-hidden="true" />
              {t("chat")}
            </Link>
            <Link
              to="/portal/attendance"
              search={{ month: undefined }}
              className="flex items-center gap-2 px-3 py-2 text-sm font-medium"
              activeOptions={{ exact: true, includeSearch: false }}
              activeProps={{ className: "border-b-2 border-primary text-primary" }}
              inactiveProps={{ className: "border-b-2 border-transparent text-muted-foreground" }}
            >
              <CalendarDays className="size-4" aria-hidden="true" />
              {t("attendance")}
            </Link>{" "}
          </nav>
          <div className="flex items-center gap-3">
            <LanguageSwitcher />
            <UserMenu me={{ ...me, displayName: name }} />
          </div>
        </header>
        {children ? (
          <main id="main-content" className="workspace flex-1 px-4 py-6 sm:px-8 sm:py-10">
            {children}
          </main>
        ) : (
          <main className="relative grid flex-1 place-items-center px-6 py-24 text-center">
            <div
              aria-hidden="true"
              className="pointer-events-none absolute inset-8 opacity-30 [background-image:linear-gradient(to_right,#8b5cf414_1px,transparent_1px),linear-gradient(to_bottom,#8b5cf414_1px,transparent_1px)] [background-size:64px_64px] [mask-image:radial-gradient(ellipse,black,transparent_70%)]"
            />
            <div className="portal-welcome relative max-w-xl space-y-6">
              <h1 className="text-4xl font-semibold tracking-tight sm:text-5xl">{t("title")}</h1>
              <p className="text-lg leading-loose text-muted-foreground">
                {t("welcome", { name: `\u2068${name}\u2069` })}
              </p>
              <div className="mx-auto h-0.5 w-16 rounded-full bg-primary" />
              <div className="flex flex-wrap justify-center gap-3 pt-3">
                <Link to="/portal/chat" className={buttonVariants({ size: "lg" })}>
                  <MessageCircle aria-hidden="true" className="size-4" />
                  {t("chat")}
                  <ArrowRight aria-hidden="true" className="size-4 rtl:rotate-180" />
                </Link>
                <Link
                  to="/portal/attendance"
                  search={{ month: undefined }}
                  className={buttonVariants({ variant: "outline", size: "lg" })}
                >
                  <CalendarDays aria-hidden="true" className="size-4" />
                  {t("attendance")}
                </Link>
              </div>
            </div>
          </main>
        )}
      </div>
    </div>
  );
}
