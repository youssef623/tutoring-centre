# `frontend/src/routes/change-password.tsx`

## Purpose

Declares `ChangePasswordPage`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/routes/change-password.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 176 lines and 7,769 bytes. Source hash: `3f9cc7943d6f577d6fe9d17b05347a476f49db200c65d0ce0647fceef8172ff2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 3: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 4: `import { useQueryClient } from "@tanstack/react-query";`
- Line 5: `import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";`
- Line 6: `import { Loader2 } from "lucide-react";`
- Line 7: `import { useForm } from "react-hook-form";`
- Line 8: `import { useTranslation } from "react-i18next";`
- Line 9: `import { toast } from "sonner";`
- Line 10: `import { asApiError } from "@/api/apiFetch";`
- Line 11: `import { refreshCsrfToken } from "@/api/csrf";`
- Line 12: `import { messageFor } from "@/api/errorMessages";`
- Line 13: `import { applyFieldErrors, showApiError } from "@/api/showApiError";`
- Line 14: `import { useChangePassword } from "@/api/generated/tutoring-centre";`
- Line 15: `import { changePasswordFormSchema, type ChangePasswordFormValues } from "@/features/auth/changePasswordSchema";`
- Line 16: `import { HessaWordmark } from "@/components/brand/HessaLogo";`
- Line 17: `import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";`
- Line 18: `import { meQueryOptions } from "@/features/session/meQueryOptions";`
- Line 19: `import { useSession } from "@/features/session/useSession";`
- Line 20: `import { Button } from "@/components/ui/button";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../frontend/src/routes/change-password.tsx#L24): `export const Route = createFileRoute("/change-password")({`
- [Line 34](../../../../../frontend/src/routes/change-password.tsx#L34): `function ChangePasswordPage() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/change-password.test.tsx:34](../../../../../frontend/src/routes/change-password.test.tsx#L34)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
/* eslint-disable @typescript-eslint/only-throw-error -- TanStack Router's documented redirect idiom:
 * `redirect()` returns a Response, which the router's own data loader catches, not an Error. */
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { refreshCsrfToken } from "@/api/csrf";
import { messageFor } from "@/api/errorMessages";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import { useChangePassword } from "@/api/generated/tutoring-centre";
import { changePasswordFormSchema, type ChangePasswordFormValues } from "@/features/auth/changePasswordSchema";
import { HessaWordmark } from "@/components/brand/HessaLogo";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { useSession } from "@/features/session/useSession";
import { Button } from "@/components/ui/button";

const KnownFields = ["currentPassword", "newPassword"] as const;

export const Route = createFileRoute("/change-password")({
  beforeLoad: async ({ context }) => {
    const me = await context.queryClient.query({ ...meQueryOptions, staleTime: "static" });
    if (me === null) {
      throw redirect({ to: "/login" });
    }
  },
  component: ChangePasswordPage,
});

function ChangePasswordPage() {
  const { t } = useTranslation("auth");
  const { me } = useSession();
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const mutation = useChangePassword();

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<ChangePasswordFormValues>({ resolver: zodResolver(changePasswordFormSchema) });

  // The guard above already resolved a signed-in user before this renders.
  if (me === null) {
    return null;
  }

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        data: { currentPassword: values.currentPassword, newPassword: values.newPassword },
      });
      // The session cookie is re-issued on a successful change, so the antiforgery token (bound to the
      // signed-in user) must be refreshed before the next unsafe request — the same pattern login and
      // centre-selection already follow.
      await refreshCsrfToken();
      await queryClient.invalidateQueries({ queryKey: meQueryOptions.queryKey });
      toast.success(t("changePassword.toastSuccess"));
      await navigate({ to: me.activeCentreId === null ? "/select-centre" : "/" });
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.code === "auth.current_password_invalid") {
        setError("currentPassword", { type: "server", message: messageFor(apiError) });
        return;
      }

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
    }
  });

  return (
    <main className="flex min-h-dvh flex-col bg-muted/30">
      <header className="flex flex-wrap items-center justify-between gap-3 px-6 py-5 sm:px-10">
        <HessaWordmark />
        <LanguageSwitcher />
      </header>

      <div className="mx-auto flex w-full max-w-sm flex-1 flex-col justify-center gap-6 px-4 pb-16 sm:px-0">
        <div className="space-y-1.5">
```

The excerpt stops at line 90; use the original file for the remaining 86 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
