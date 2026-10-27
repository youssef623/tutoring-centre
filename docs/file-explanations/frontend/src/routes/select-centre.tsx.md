# `frontend/src/routes/select-centre.tsx`

## Purpose

Source comment (intent): Role badge styling, matched to the role's existing translation key (`auth:roles.*`). Decorative color beyond the badge's own text/icon — never the only signal of which role this is.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/routes/select-centre.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 268 lines and 10,311 bytes. Source hash: `7c77f704d17128460386c3a1edba3902b034753b946f9af54ec44c12190b12ba`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 3: `import { useQueryClient } from "@tanstack/react-query";`
- Line 4: `import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";`
- Line 5: `import {`
- Line 17: `import type { LucideIcon } from "lucide-react";`
- Line 18: `import { useState } from "react";`
- Line 19: `import { useTranslation } from "react-i18next";`
- Line 20: `import { cn } from "cn";`
- Line 21: `import { asApiError } from "@/api/apiFetch";`
- Line 22: `import { refreshCsrfToken } from "@/api/csrf";`
- Line 23: `import { showApiError } from "@/api/showApiError";`
- Line 24: `import { useSelectCentre, type MembershipDto } from "@/api/generated/tutoring-centre";`
- Line 25: `import { HessaWordmark } from "@/components/brand/HessaLogo";`
- Line 26: `import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";`
- Line 27: `import { meQueryOptions } from "@/features/session/meQueryOptions";`
- Line 28: `import { useSession } from "@/features/session/useSession";`
- Line 29: `import { useSignOut } from "@/features/session/useSignOut";`
- Line 30: `import { initialsOf } from "@/lib/initials";`
- Line 31: `import { Button } from "@/components/ui/button";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 33](../../../../../frontend/src/routes/select-centre.tsx#L33): `export const Route = createFileRoute("/select-centre")({`
- [Line 64](../../../../../frontend/src/routes/select-centre.tsx#L64): `function avatarClassName(centreId: string): string {`
- [Line 73](../../../../../frontend/src/routes/select-centre.tsx#L73): `function SelectCentrePage() {`
- [Line 150](../../../../../frontend/src/routes/select-centre.tsx#L150): `function NoActiveCentre({ email, onSignOut }: { email: string; onSignOut: () => Promise<void> }) {`
- [Line 223](../../../../../frontend/src/routes/select-centre.tsx#L223): `function CentreCard({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/select-centre.test.tsx:27](../../../../../frontend/src/routes/select-centre.test.tsx#L27)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

The excerpt stops at line 90; use the original file for the remaining 178 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
