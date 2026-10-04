/* eslint-disable @typescript-eslint/only-throw-error -- TanStack Router's documented redirect idiom:
 * `redirect()` returns a Response, which the router's own data loader catches, not an Error. */
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { refreshCsrfToken } from "@/api/csrf";
import { showApiError } from "@/api/showApiError";
import { useLogout, useSelectCentre } from "@/api/generated/tutoring-centre";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { useSession } from "@/features/session/useSession";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";

export const Route = createFileRoute("/select-centre")({
  beforeLoad: async ({ context }) => {
    const me = await context.queryClient.query({ ...meQueryOptions, staleTime: "static" });
    if (me === null) {
      throw redirect({ to: "/login" });
    }
  },
  component: SelectCentrePage,
});

function SelectCentrePage() {
  const { t } = useTranslation("auth");
  const { me } = useSession();
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const selectCentreMutation = useSelectCentre();
  const logoutMutation = useLogout();

  // The guard above already redirected a signed-out visitor; this only covers the render before that settles.
  if (me === null) {
    return null;
  }

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

  const handleLogout = async () => {
    await logoutMutation.mutateAsync();
    await refreshCsrfToken();
    queryClient.clear();
    await navigate({ to: "/login" });
  };

  return (
    <main className="mx-auto flex min-h-screen w-full max-w-md flex-col justify-center gap-4 p-6">
      <div className="flex justify-end">
        <LanguageSwitcher />
      </div>
      {me.memberships.length === 0 ? (
        <div className="space-y-4 text-center">
          <p className="text-sm text-muted-foreground">{t("selectCentre.empty")}</p>
          <Button
            type="button"
            variant="outline"
            onClick={() => {
              void handleLogout();
            }}
          >
            {t("selectCentre.logout")}
          </Button>
        </div>
      ) : (
        <div className="space-y-4">
          <div className="space-y-1 text-center">
            <h1 className="text-2xl font-semibold">{t("selectCentre.title")}</h1>
            <p className="text-sm text-muted-foreground">{t("selectCentre.subtitle")}</p>
          </div>
          <div className="space-y-2">
            {me.memberships.map((membership) => (
              <button
                key={membership.centreId}
                type="button"
                className="flex w-full items-center justify-between gap-2 rounded-xl border border-border bg-card p-3 text-start text-card-foreground outline-none hover:bg-muted focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"
                onClick={() => {
                  void handleSelect(membership.centreId);
                }}
              >
                <span className="font-medium">{membership.centreName}</span>
                <Badge variant="secondary">{t(`roles.${String(membership.role)}`)}</Badge>
              </button>
            ))}
          </div>
        </div>
      )}
    </main>
  );
}
