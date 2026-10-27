# `frontend/src/features/shell/AppShell.tsx`

## Purpose

Source comment (intent): The product's name and mark, linking home; the same lockup as the sign-in page, sized for the sidebar.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/shell/AppShell.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 231 lines and 9,718 bytes. Source hash: `168240628798c3e1c5b7cf01ec640143b4fca521c6534a103ac7d919478a7459`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Dialog } from "@base-ui/react/dialog";`
- Line 2: `import { Menu } from "@base-ui/react/menu";`
- Line 3: `import { Link, useMatches } from "@tanstack/react-router";`
- Line 4: `import { cn } from "cn";`
- Line 5: `import {`
- Line 18: `import type { ReactNode } from "react";`
- Line 19: `import { useState } from "react";`
- Line 20: `import { useTranslation } from "react-i18next";`
- Line 21: `import { HessaWordmark } from "@/components/brand/HessaLogo";`
- Line 22: `import { Badge } from "@/components/ui/badge";`
- Line 23: `import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";`
- Line 24: `import type { MeDto } from "@/api/generated/tutoring-centre";`
- Line 25: `import { Can } from "@/features/session/Can";`
- Line 26: `import { Permissions } from "@/features/session/permissions";`
- Line 27: `import { useSignOut } from "@/features/session/useSignOut";`
- Line 28: `import { initialsOf } from "@/lib/initials";`
- Line 29: `import "@/features/shell/routeStaticData";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 31](../../../../../../frontend/src/features/shell/AppShell.tsx#L31): `export function AppShell({ me, children }: { me: MeDto; children: ReactNode }) {`
- [Line 101](../../../../../../frontend/src/features/shell/AppShell.tsx#L101): `function SidebarBrand({ onNavigate }: { onNavigate?: () => void }) {`
- [Line 116](../../../../../../frontend/src/features/shell/AppShell.tsx#L116): `function SidebarNav({ onNavigate }: { onNavigate?: () => void }) {`
- [Line 180](../../../../../../frontend/src/features/shell/AppShell.tsx#L180): `function UserMenu({ me }: { me: MeDto }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated.tsx:4](../../../../../../frontend/src/routes/_authenticated.tsx#L4)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Dialog } from "@base-ui/react/dialog";
import { Menu } from "@base-ui/react/menu";
import { Link, useMatches } from "@tanstack/react-router";
import { cn } from "cn";
import {
  BookOpen,
  CalendarRange,
  GraduationCap,
  History,
  LayoutDashboard,
  Menu as MenuIcon,
  Presentation,
  Settings,
  Users,
  UsersRound,
  X,
} from "lucide-react";
import type { ReactNode } from "react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { HessaWordmark } from "@/components/brand/HessaLogo";
import { Badge } from "@/components/ui/badge";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import type { MeDto } from "@/api/generated/tutoring-centre";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useSignOut } from "@/features/session/useSignOut";
import { initialsOf } from "@/lib/initials";
import "@/features/shell/routeStaticData";

export function AppShell({ me, children }: { me: MeDto; children: ReactNode }) {
  const { t: tShell } = useTranslation("shell");
  const { t: tAuth } = useTranslation("auth");
  const [mobileNavOpen, setMobileNavOpen] = useState(false);
  const activeCentre = me.memberships.find((membership) => membership.centreId === me.activeCentreId);
  // Every page reads at max-w-5xl; a route asks for more with `staticData.layout` (the timetable's seven columns).
  const isWide = useMatches({ select: (matches) => matches.some((match) => match.staticData.layout === "wide") });

  return (
    <Dialog.Root open={mobileNavOpen} onOpenChange={setMobileNavOpen}>
      <div className="flex min-h-screen">
        <aside className="hidden w-56 shrink-0 space-y-6 border-e border-sidebar-border bg-sidebar p-4 text-sidebar-foreground md:block">
          <SidebarBrand />
          <SidebarNav />
        </aside>

        <Dialog.Portal>
          <Dialog.Backdrop className="fixed inset-0 bg-foreground/40 transition-opacity data-[ending-style]:opacity-0 data-[starting-style]:opacity-0" />
          <Dialog.Popup className="fixed inset-y-0 start-0 flex h-full w-64 flex-col gap-4 bg-sidebar p-4 text-sidebar-foreground shadow-md transition-transform ltr:data-[ending-style]:-translate-x-full ltr:data-[starting-style]:-translate-x-full rtl:data-[ending-style]:translate-x-full rtl:data-[starting-style]:translate-x-full">
            <div className="flex items-center justify-between gap-2">
              <SidebarBrand
                onNavigate={() => {
                  setMobileNavOpen(false);
                }}
              />
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

        <div className="flex min-w-0 flex-1 flex-col">
          <header className="flex items-center justify-between gap-3 border-b border-border px-4 py-3 md:px-6">
            <div className="flex min-w-0 items-center gap-2">
              <Dialog.Trigger
                className="rounded-md p-1.5 outline-none hover:bg-muted focus-visible:ring-3 focus-visible:ring-ring/50 md:hidden"
                aria-label={tShell("nav.openMenu")}
              >
                <MenuIcon className="size-5" aria-hidden="true" />
              </Dialog.Trigger>
              {activeCentre !== undefined && (
                <div aria-label={tShell("header.activeCentre")} className="flex min-w-0 items-center gap-2">
                  <span dir="auto" className="truncate font-medium">
                    {activeCentre.centreName}
                  </span>
                  <Badge variant="secondary">{tAuth(`roles.${String(activeCentre.role)}`)}</Badge>
                </div>
              )}
            </div>
            <UserMenu me={me} />
          </header>
```

The excerpt stops at line 90; use the original file for the remaining 141 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
