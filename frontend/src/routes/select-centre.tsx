/* eslint-disable @typescript-eslint/only-throw-error -- TanStack Router's documented redirect idiom:
 * `redirect()` returns a Response, which the router's own data loader catches, not an Error. */
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { Inbox } from "lucide-react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { refreshCsrfToken } from "@/api/csrf";
import { showApiError } from "@/api/showApiError";
import { useSelectCentre } from "@/api/generated/tutoring-centre";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import { useSession } from "@/features/session/useSession";
import { useSignOut } from "@/features/session/useSignOut";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

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
  const signOut = useSignOut();

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

  return (
    <main className="flex min-h-screen items-center justify-center bg-muted/40 p-6">
      <div className="w-full max-w-md space-y-4">
        <div className="flex justify-end">
          <LanguageSwitcher />
        </div>
        {me.memberships.length === 0 ? (
          <Card className="shadow-sm">
            <CardContent className="flex flex-col items-center gap-4 py-6 text-center">
              <Inbox className="size-10 text-muted-foreground" aria-hidden="true" />
              <p className="text-sm text-muted-foreground">{t("selectCentre.empty")}</p>
              <Button
                type="button"
                variant="outline"
                onClick={() => {
                  void signOut();
                }}
              >
                {t("selectCentre.logout")}
              </Button>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-4">
            <div className="space-y-1 text-center">
              <h1 className="text-2xl font-semibold tracking-tight">{t("selectCentre.title")}</h1>
              <p className="text-sm text-muted-foreground">{t("selectCentre.subtitle")}</p>
            </div>
            <div className="space-y-2">
              {me.memberships.map((membership) => (
                <button
                  key={membership.centreId}
                  type="button"
                  className="flex w-full items-center justify-between gap-2 rounded-xl border border-border bg-card p-4 text-start text-card-foreground shadow-sm outline-none transition-colors hover:border-ring/50 hover:bg-muted focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"
                  onClick={() => {
                    void handleSelect(membership.centreId);
                  }}
                >
                  <span className="font-medium" dir="auto">
                    {membership.centreName}
                  </span>
                  <Badge variant="secondary">{t(`roles.${String(membership.role)}`)}</Badge>
                </button>
              ))}
            </div>
          </div>
        )}
      </div>
    </main>
  );
}
