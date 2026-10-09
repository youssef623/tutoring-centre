import { Dialog } from "@base-ui/react/dialog";
import { Menu } from "@base-ui/react/menu";
import { Link } from "@tanstack/react-router";
import { BookOpen, LayoutDashboard, Menu as MenuIcon, X } from "lucide-react";
import type { ReactNode } from "react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Badge } from "@/components/ui/badge";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import type { MeDto } from "@/api/generated/tutoring-centre";
import { useSignOut } from "@/features/session/useSignOut";

/** First letter of up to the first two words, for the user-menu avatar. Falls back to "?" for an empty name. */
function initialsOf(displayName: string): string {
  const initials = displayName
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((word) => word.charAt(0).toUpperCase())
    .join("");

  return initials === "" ? "?" : initials;
}

export function AppShell({ me, children }: { me: MeDto; children: ReactNode }) {
  const { t: tShell } = useTranslation("shell");
  const { t: tAuth } = useTranslation("auth");
  const [mobileNavOpen, setMobileNavOpen] = useState(false);
  const activeCentre = me.memberships.find((membership) => membership.centreId === me.activeCentreId);

  return (
    <Dialog.Root open={mobileNavOpen} onOpenChange={setMobileNavOpen}>
      <div className="flex min-h-screen">
        <aside className="hidden w-56 shrink-0 border-e border-sidebar-border bg-sidebar p-4 text-sidebar-foreground md:block">
          <SidebarNav />
        </aside>

        <Dialog.Portal>
          <Dialog.Backdrop className="fixed inset-0 bg-foreground/40 transition-opacity data-[ending-style]:opacity-0 data-[starting-style]:opacity-0" />
          <Dialog.Popup className="fixed inset-y-0 start-0 flex h-full w-64 flex-col gap-4 bg-sidebar p-4 text-sidebar-foreground shadow-md transition-transform ltr:data-[ending-style]:-translate-x-full ltr:data-[starting-style]:-translate-x-full rtl:data-[ending-style]:translate-x-full rtl:data-[starting-style]:translate-x-full">
            <div className="flex justify-end">
              <Dialog.Close
                className="rounded-md p-1.5 outline-none hover:bg-sidebar-accent focus-visible:ring-3 focus-visible:ring-ring/50"
                aria-label={tShell("nav.closeMenu")}
              >
                <X className="size-4" aria-hidden="true" />
              </Dialog.Close>
            </div>
            <SidebarNav
              onNavigate={() => {
                setMobileNavOpen(false);
              }}
            />
          </Dialog.Popup>
        </Dialog.Portal>

        <div className="flex flex-1 flex-col">
          <header className="flex items-center justify-between gap-4 border-b border-border px-4 py-3 md:px-6">
            <div className="flex items-center gap-2">
              <Dialog.Trigger
                className="rounded-md p-1.5 outline-none hover:bg-muted focus-visible:ring-3 focus-visible:ring-ring/50 md:hidden"
                aria-label={tShell("nav.openMenu")}
              >
                <MenuIcon className="size-5" aria-hidden="true" />
              </Dialog.Trigger>
              {activeCentre !== undefined && (
                <div aria-label={tShell("header.activeCentre")} className="flex items-center gap-2">
                  <span dir="auto" className="font-medium">
                    {activeCentre.centreName}
                  </span>
                  <Badge variant="secondary">{tAuth(`roles.${String(activeCentre.role)}`)}</Badge>
                </div>
              )}
            </div>
            <UserMenu me={me} />
          </header>
          <main className="flex-1 p-4 md:p-6">
            <div className="mx-auto w-full max-w-5xl">{children}</div>
          </main>
        </div>
      </div>
    </Dialog.Root>
  );
}

function SidebarNav({ onNavigate }: { onNavigate?: () => void }) {
  const { t } = useTranslation("shell");

  const linkClassName =
    "flex items-center gap-2.5 rounded-md border-s-2 border-s-transparent px-3 py-2 text-sm font-medium transition-colors hover:bg-sidebar-accent hover:text-sidebar-accent-foreground data-[status=active]:border-s-sidebar-primary data-[status=active]:bg-sidebar-accent data-[status=active]:text-sidebar-accent-foreground";

  return (
    <nav className="space-y-1">
      <Link to="/" className={linkClassName} activeOptions={{ exact: true }} onClick={onNavigate}>
        <LayoutDashboard className="size-4" aria-hidden="true" />
        {t("nav.dashboard")}
      </Link>
      <Link to="/subjects" search={{ archived: false }} className={linkClassName} onClick={onNavigate}>
        <BookOpen className="size-4" aria-hidden="true" />
        {t("nav.subjects")}
      </Link>
    </nav>
  );
}

function UserMenu({ me }: { me: MeDto }) {
  const { t } = useTranslation("shell");
  const signOut = useSignOut();
  const canSwitchCentre = me.memberships.length > 1;

  return (
    <Menu.Root>
      <Menu.Trigger className="flex items-center gap-2 rounded-md py-1.5 ps-1.5 pe-3 text-sm font-medium outline-none hover:bg-muted focus-visible:ring-3 focus-visible:ring-ring/50">
        <span
          className="flex size-7 shrink-0 items-center justify-center rounded-full bg-primary text-xs font-medium text-primary-foreground"
          aria-hidden="true"
        >
          {initialsOf(me.displayName)}
        </span>
        <span dir="auto">{me.displayName}</span>
      </Menu.Trigger>
      <Menu.Portal>
        <Menu.Positioner align="end" sideOffset={4}>
          <Menu.Popup className="min-w-44 rounded-lg border border-border bg-popover p-1 text-popover-foreground shadow-md">
            {canSwitchCentre && (
              <Menu.Item
                className="cursor-pointer rounded-md px-3 py-1.5 text-sm outline-none data-[highlighted]:bg-muted"
                render={<Link to="/select-centre" />}
              >
                {t("userMenu.switchCentre")}
              </Menu.Item>
            )}
            <div className="px-3 py-1.5">
              <LanguageSwitcher />
            </div>
            <Menu.Item
              className="cursor-pointer rounded-md px-3 py-1.5 text-sm outline-none data-[highlighted]:bg-muted"
              onClick={() => {
                void signOut();
              }}
            >
              {t("userMenu.logout")}
            </Menu.Item>
          </Menu.Popup>
        </Menu.Positioner>
      </Menu.Portal>
    </Menu.Root>
  );
}
