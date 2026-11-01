import { ArrowRight, House, CalendarDays, MessageCircle } from "lucide-react";
import { Link } from "@tanstack/react-router";
import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import type { MeDto } from "@/api/generated/tutoring-centre";
import { HessaWordmark } from "@/components/brand/HessaLogo";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { UserMenu } from "@/features/shell/AppShell";

export function PortalShell({ me, children }: { me: MeDto; children?: ReactNode }) {
  const { t } = useTranslation("portal");
  const centre = me.memberships.find((item) => item.centreId === me.activeCentreId);
  const name = me.portal?.subjectName ?? me.displayName;
  const [welcomeStart, welcomeEnd] = t("homeWelcome", { name: "__name__" }).split("__name__");
  return (
    <div className="portal-shell min-h-dvh w-full">
      <div className="flex min-h-dvh w-full flex-col overflow-hidden">
        <header className="portal-header flex flex-wrap items-center justify-between gap-5 border-b border-primary/15 bg-card px-5 py-5 sm:px-8">
          <div className="space-y-1">
            <Link to="/portal" aria-label={t("shell:nav.home")}>
              <HessaWordmark />
            </Link>
            <p className="text-sm text-muted-foreground" dir="auto">
              {centre?.centreName}
            </p>
          </div>
          <nav
            aria-label={t("navigation")}
            className="order-3 flex w-full justify-center gap-6 border-t border-border pt-4 md:order-0 md:w-auto md:border-0 md:pt-0"
          >
            <Link
              to="/portal"
              className="flex items-center gap-2 px-3 py-2 text-sm font-medium"
              activeOptions={{ exact: true }}
            >
              <House className="size-4" aria-hidden="true" />
              {t("home")}
            </Link>
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
          <main className="portal-home" id="main-content">
            <img className="learning-fan" src="/brand/learning-fan.png" alt="" />
            <h1 className="portal-context-title">{t("title")}</h1>
            <h2 className="portal-welcome-heading">
              {welcomeStart}
              <span>{name}</span>
              {welcomeEnd}
            </h2>
            <p>{t("homePrompt")}</p>
            <div className="portal-paths">
              <Link className="portal-path" to="/portal/attendance" search={{ month: undefined }}>
                <img
                  className="portal-path-illustration"
                  src="/brand/attendance-calendar.png"
                  alt=""
                  decoding="async"
                />
                <CalendarDays aria-hidden="true" />
                <h2>{t("attendance")}</h2>
                <p>
                  {t(
                    me.portal?.kind === "parent" ? "homeAttendanceParent" : "homeAttendanceStudent",
                  )}
                </p>
                <span className="path-action">
                  {t("viewAttendance")}
                  <ArrowRight aria-hidden="true" className="rtl:rotate-180" />
                </span>
              </Link>
              <Link className="portal-path" to="/portal/chat">
                <img
                  className="portal-path-illustration"
                  src="/brand/chat-bubbles.png"
                  alt=""
                  decoding="async"
                />
                <MessageCircle aria-hidden="true" />
                <h2>{t("chat")}</h2>
                <p>{t("chatEmpty")}</p>
                <span className="path-action">
                  {t("openChat")}
                  <ArrowRight aria-hidden="true" className="rtl:rotate-180" />
                </span>
              </Link>
            </div>
          </main>
        )}
      </div>
    </div>
  );
}
