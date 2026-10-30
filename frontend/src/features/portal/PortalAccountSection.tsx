import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import {
  createPortalAccount,
  deactivatePortalAccount,
  reactivatePortalAccount,
  listPortalAccounts,
  type PortalSubjectKind,
} from "@/api/generated/tutoring-centre";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";

export function PortalAccountSection(props: { kind: PortalSubjectKind; subjectId: string }) {
  return (
    <Can permission={Permissions.PortalAccountsManage}>
      <AccountSection key={`${props.kind}:${props.subjectId}`} {...props} />
    </Can>
  );
}

function AccountSection({ kind, subjectId }: { kind: PortalSubjectKind; subjectId: string }) {
  const { t } = useTranslation("portal");
  const queryClient = useQueryClient();
  const queryKey = ["portal-accounts", kind, subjectId];
  const query = useQuery({ queryKey, queryFn: () => listPortalAccounts({ kind, subjectId }) });
  const [open, setOpen] = useState(false);
  const [password, setPassword] = useState<string | null>(null);
  const [created, setCreated] = useState(false);
  const [pending, setPending] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [copied, setCopied] = useState(false);
  const account = query.data?.items[0];
  const close = () => {
    setOpen(false);
    setPassword(null);
    setCreated(false);
    setCopied(false);
    setError(null);
  };
  const failure = (thrown: unknown) => {
    const apiError = asApiError(thrown);
    setError(
      apiError.code === "portal.subject_already_linked"
        ? t("subjectLinked")
        : apiError.code === "portal.user_already_linked"
          ? t("userLinked")
          : messageFor(apiError),
    );
  };
  const toggle = async () => {
    if (account === undefined) return;
    setPending(true);
    setError(null);
    try {
      await (account.status === "active" ? deactivatePortalAccount : reactivatePortalAccount)(
        account.linkId,
        { version: account.version },
      );
      await queryClient.invalidateQueries({ queryKey });
    } catch (thrown) {
      failure(thrown);
    } finally {
      setPending(false);
    }
  };
  return (
    <section
      className="space-y-4 rounded-xl border border-border p-5"
      aria-label={t("accountTitle")}
    >
      <div>
        <h2 className="text-lg font-semibold">{t("accountTitle")}</h2>
        <p className="mt-1 text-sm text-muted-foreground">{t("accountDescription")}</p>
      </div>
      {query.isPending ? (
        <p role="status">{t("loading")}</p>
      ) : query.isError ? (
        <div role="alert">
          <p>{messageFor(asApiError(query.error))}</p>
          <Button
            variant="outline"
            onClick={() => {
              void query.refetch();
            }}
          >
            {t("retry")}
          </Button>
        </div>
      ) : (
        <div className="flex flex-wrap items-center justify-between gap-4">
          <div>
            {account === undefined ? (
              <p className="text-sm text-muted-foreground">{t("none")}</p>
            ) : (
              <>
                <p className="break-all text-sm" dir="ltr">
                  {account.email}
                </p>
                <p className="mt-1 text-sm text-muted-foreground">
                  {t(account.status === "active" ? "active" : "inactive")}
                </p>
              </>
            )}
          </div>
          {account === undefined ? (
            <Button
              onClick={() => {
                setOpen(true);
              }}
            >
              {t("create")}
            </Button>
          ) : (
            <Button
              variant="outline"
              disabled={pending}
              onClick={() => {
                void toggle();
              }}
            >
              {t(account.status === "active" ? "deactivate" : "reactivate")}
            </Button>
          )}
        </div>
      )}
      {error !== null && !open && (
        <p role="alert" className="text-sm text-destructive">
          {error}
        </p>
      )}
      <Dialog
        open={open}
        onOpenChange={(value) => {
          if (!value && !pending) close();
        }}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{t(created ? "created" : "createTitle")}</DialogTitle>
            <DialogDescription>
              {t(
                created
                  ? password === null
                    ? "existingAccount"
                    : "passwordWarning"
                  : "createDescription",
              )}
            </DialogDescription>
          </DialogHeader>
          {created ? (
            <div className="space-y-4">
              {password !== null && (
                <>
                  <Label>{t("temporaryPassword")}</Label>
                  <output
                    className="block rounded-lg border border-border bg-muted p-4 font-mono text-lg"
                    dir="ltr"
                  >
                    {password}
                  </output>
                  <Button
                    variant="outline"
                    onClick={() => {
                      const copy = async () => {
                        try {
                          await navigator.clipboard.writeText(password);
                          setCopied(true);
                          setError(null);
                        } catch {
                          setError(t("copyFailed"));
                        }
                      };
                      void copy();
                    }}
                  >
                    {t(copied ? "copied" : "copy")}
                  </Button>
                </>
              )}
              {error !== null && (
                <p role="alert" className="text-sm text-destructive">
                  {error}
                </p>
              )}
              <Button className="w-full" onClick={close}>
                {t("done")}
              </Button>
            </div>
          ) : (
            <form
              className="space-y-4"
              onSubmit={(event) => {
                event.preventDefault();
                const form = new FormData(event.currentTarget);
                const email = form.get("email");
                if (typeof email !== "string") return;
                setPending(true);
                setError(null);
                // Call the API directly: one-time credentials must never enter a mutation cache.
                void createPortalAccount({
                  kind,
                  subjectId,
                  email,
                  preferredLocale: form.get("locale") === "en" ? "en" : "ar",
                })
                  .then(async (result) => {
                    setPassword(result.temporaryPassword ?? null);
                    setCreated(true);
                    await queryClient.invalidateQueries({ queryKey });
                  })
                  .catch(failure)
                  .finally(() => {
                    setPending(false);
                  });
              }}
            >
              <div className="space-y-2">
                <Label htmlFor={`portal-email-${subjectId}`}>{t("email")}</Label>
                <Input
                  id={`portal-email-${subjectId}`}
                  name="email"
                  type="email"
                  maxLength={256}
                  required
                  autoComplete="off"
                  dir="ltr"
                  disabled={pending}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor={`portal-locale-${subjectId}`}>{t("language")}</Label>
                <select
                  id={`portal-locale-${subjectId}`}
                  name="locale"
                  defaultValue="ar"
                  disabled={pending}
                  className="h-9 w-full rounded-md border border-input bg-background px-3 text-sm"
                >
                  <option value="ar">العربية</option>
                  <option value="en">English</option>
                </select>
              </div>
              {error !== null && (
                <p role="alert" className="text-sm text-destructive">
                  {error}
                </p>
              )}
              <div className="flex justify-end gap-2">
                <Button type="button" variant="outline" disabled={pending} onClick={close}>
                  {t("cancel")}
                </Button>
                <Button type="submit" disabled={pending}>
                  {t("create")}
                </Button>
              </div>
            </form>
          )}
        </DialogContent>
      </Dialog>
    </section>
  );
}
