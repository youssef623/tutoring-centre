import { Link } from "@tanstack/react-router";
import { ShieldAlert } from "lucide-react";
import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { useCan } from "./useCan";
import type { Permission } from "./permissions";

/**
 * Route-level permission guard: renders its children only when the session holds the given permission,
 * otherwise a translated "no access" state with a link back to the dashboard. Because the protected page's
 * own data hooks live inside `children`, which never mounts when the permission is missing, no request for
 * the protected data is ever issued — this is usability only, never the security boundary (the API is).
 */
export function RouteGuard({ permission, children }: { permission: Permission; children: ReactNode }) {
  const { t } = useTranslation("shell");
  const allowed = useCan(permission);

  if (!allowed) {
    return (
      <div className="flex flex-col items-center gap-3 py-16 text-center">
        <ShieldAlert className="size-10 text-muted-foreground" aria-hidden="true" />
        <h1 className="text-xl font-semibold tracking-tight">{t("noAccess.title")}</h1>
        <p className="max-w-sm text-sm text-muted-foreground">{t("noAccess.description")}</p>
        <Button render={<Link to="/" />}>{t("noAccess.backToDashboard")}</Button>
      </div>
    );
  }

  return <>{children}</>;
}
