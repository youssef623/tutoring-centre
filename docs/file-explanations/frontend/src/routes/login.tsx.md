# `frontend/src/routes/login.tsx`

## Purpose

Source comment (intent): Only ever a relative, same-site path: an open `redirect` search parameter is never trusted as-is.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/routes/login.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 226 lines and 9,724 bytes. Source hash: `dd6042853e035000dbb63834f6e221bac43593ed4eeee557e506247499ac4a55`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { useQueryClient } from "@tanstack/react-query";`
- Line 3: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- Line 4: `import { ClipboardCheck, Loader2, MessageCircle, Users, Wallet } from "lucide-react";`
- Line 5: `import type { LucideIcon } from "lucide-react";`
- Line 6: `import { useState } from "react";`
- Line 7: `import { useForm } from "react-hook-form";`
- Line 8: `import { useTranslation } from "react-i18next";`
- Line 9: `import { z } from "zod";`
- Line 10: `import { applyFieldErrors } from "@/api/showApiError";`
- Line 11: `import { messageFor } from "@/api/errorMessages";`
- Line 12: `import { refreshCsrfToken } from "@/api/csrf";`
- Line 13: `import { asApiError } from "@/api/apiFetch";`
- Line 14: `import { useLogin, type MeDto } from "@/api/generated/tutoring-centre";`
- Line 15: `import { meQueryOptions } from "@/features/session/meQueryOptions";`
- Line 16: `import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";`
- Line 17: `import { HessaMark, HessaWordmark } from "@/components/brand/HessaLogo";`
- Line 18: `import { Alert, AlertDescription } from "@/components/ui/alert";`
- Line 19: `import { Button } from "@/components/ui/button";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../frontend/src/routes/login.tsx#L21): `export interface LoginSearch {`
- [Line 25](../../../../../frontend/src/routes/login.tsx#L25): `export const Route = createFileRoute("/login")({`
- [Line 48](../../../../../frontend/src/routes/login.tsx#L48): `function resolveTarget(me: MeDto, redirect: string | undefined): string {`
- [Line 60](../../../../../frontend/src/routes/login.tsx#L60): `function LoginPage() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/login.test.tsx:13](../../../../../frontend/src/routes/login.test.tsx#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
import { useLogin, type MeDto } from "@/api/generated/tutoring-centre";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { HessaMark, HessaWordmark } from "@/components/brand/HessaLogo";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";

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

const PANEL_FEATURES: { icon: LucideIcon; key: "attendance" | "groups" | "fees" | "assistant" }[] = [
  { icon: ClipboardCheck, key: "attendance" },
  { icon: Users, key: "groups" },
  { icon: Wallet, key: "fees" },
  { icon: MessageCircle, key: "assistant" },
];

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
```

The excerpt stops at line 90; use the original file for the remaining 136 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
