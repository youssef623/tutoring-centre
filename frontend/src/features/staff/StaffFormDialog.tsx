import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Check, Copy, Loader2 } from "lucide-react";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import { getListStaffQueryKey, useCreateStaff, StaffRole } from "@/api/generated/tutoring-centre";
import { createStaffFormSchema, type CreateStaffFormValues } from "@/features/staff/staffSchema";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const KnownFields = ["email", "displayName"] as const;

/**
 * Create-member dialog, followed by the one-time password panel. The temporary password lives only in this
 * component's own state (`revealed`), set directly from the mutation's resolved value — never read back from
 * the mutation's own cached `data` — and is discarded the moment the dialog closes: nothing is retained in the
 * query cache, the URL, storage, a toast or a log.
 */
export function StaffFormDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {
  const { t } = useTranslation("staff");
  const queryClient = useQueryClient();
  const createMutation = useCreateStaff();
  const [revealed, setRevealed] = useState<{ displayName: string; password: string } | null>(null);
  const [existingAccountName, setExistingAccountName] = useState<string | null>(null);
  const [copied, setCopied] = useState(false);

  const {
    register,
    control,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateStaffFormValues>({
    resolver: zodResolver(createStaffFormSchema),
    defaultValues: { email: "", displayName: "", role: StaffRole.teacher, preferredLocale: "en" },
  });

  const closeAndReset = () => {
    onOpenChange(false);
    reset({ email: "", displayName: "", role: StaffRole.teacher, preferredLocale: "en" });
    setRevealed(null);
    setExistingAccountName(null);
    setCopied(false);
    createMutation.reset();
  };

  const onSubmit = handleSubmit(async (values) => {
    try {
      const result = await createMutation.mutateAsync({ data: values });
      await queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });

      if (result.temporaryPassword !== null) {
        setRevealed({ displayName: values.displayName, password: result.temporaryPassword });
      } else {
        setExistingAccountName(values.displayName);
      }

      toast.success(t("toasts.created"));
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    }
  });

  const handleCopy = async () => {
    if (revealed === null) {
      return;
    }

    try {
      await navigator.clipboard.writeText(revealed.password);
      setCopied(true);
      setTimeout(() => {
        setCopied(false);
      }, 2000);
    } catch {
      // Clipboard access can be denied; the password is still selectable as text.
    }
  };

  return (
    <Dialog
      open={open}
      onOpenChange={(next) => {
        if (!next) {
          closeAndReset();
        }
      }}
    >
      <DialogContent>
        {revealed !== null ? (
          <>
            <DialogHeader>
              <DialogTitle>{t("password.title")}</DialogTitle>
              <DialogDescription>{t("password.description", { name: revealed.displayName })}</DialogDescription>
            </DialogHeader>
            <div className="py-2">
              <div
                dir="ltr"
                className="flex items-center justify-between gap-2 rounded-lg border border-border bg-muted px-3 py-2 font-mono text-sm"
              >
                <span className="select-all">{revealed.password}</span>
                <Button type="button" variant="outline" size="sm" onClick={() => void handleCopy()}>
                  {copied ? <Check aria-hidden="true" /> : <Copy aria-hidden="true" />}
                  {copied ? t("password.copied") : t("password.copy")}
                </Button>
              </div>
            </div>
            <DialogFooter>
              <Button type="button" onClick={closeAndReset}>
                {t("password.done")}
              </Button>
            </DialogFooter>
          </>
        ) : existingAccountName !== null ? (
          <>
            <DialogHeader>
              <DialogTitle>{t("existingAccount.title")}</DialogTitle>
              <DialogDescription>
                {t("existingAccount.description", { name: existingAccountName })}
              </DialogDescription>
            </DialogHeader>
            <DialogFooter>
              <Button type="button" onClick={closeAndReset}>
                {t("password.done")}
              </Button>
            </DialogFooter>
          </>
        ) : (
          <form
            onSubmit={(event) => {
              void onSubmit(event);
            }}
            noValidate
          >
            <DialogHeader>
              <DialogTitle>{t("dialogs.createTitle")}</DialogTitle>
              <DialogDescription className="sr-only">{t("dialogs.createTitle")}</DialogDescription>
            </DialogHeader>
            <div className="space-y-3 py-2">
              <div className="space-y-1.5">
                <Label htmlFor="staff-email">{t("form.emailLabel")}</Label>
                <Input
                  id="staff-email"
                  type="email"
                  dir="ltr"
                  autoFocus
                  disabled={isSubmitting}
                  placeholder={t("form.emailPlaceholder")}
                  aria-invalid={errors.email !== undefined}
                  {...register("email")}
                />
                {errors.email !== undefined && (
                  <p className="text-sm text-destructive">
                    {errors.email.type === "server" ? errors.email.message : t(errors.email.message ?? "")}
                  </p>
                )}
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="staff-display-name">{t("form.displayNameLabel")}</Label>
                <Input
                  id="staff-display-name"
                  disabled={isSubmitting}
                  placeholder={t("form.displayNamePlaceholder")}
                  aria-invalid={errors.displayName !== undefined}
                  {...register("displayName")}
                />
                {errors.displayName !== undefined && (
                  <p className="text-sm text-destructive">
                    {errors.displayName.type === "server"
                      ? errors.displayName.message
                      : t(errors.displayName.message ?? "")}
                  </p>
                )}
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="staff-role">{t("form.roleLabel")}</Label>
                <RoleField control={control} disabled={isSubmitting} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="staff-locale">{t("form.preferredLocaleLabel")}</Label>
                <LocaleField control={control} disabled={isSubmitting} />
              </div>
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={closeAndReset}>
                {t("form.cancel")}
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
                {t("form.save")}
              </Button>
            </DialogFooter>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}

/** Wired through RHF's `Controller`, not `register`: the Base UI Select is not a native form control. */
function RoleField({
  control,
  disabled,
}: {
  control: ReturnType<typeof useForm<CreateStaffFormValues>>["control"];
  disabled: boolean;
}) {
  const { t } = useTranslation("auth");

  return (
    <Controller
      control={control}
      name="role"
      render={({ field }) => (
        <Select value={field.value} onValueChange={field.onChange} disabled={disabled}>
          <SelectTrigger id="staff-role" className="w-full">
            <SelectValue>{(value: CreateStaffFormValues["role"]) => t(`roles.${value}`)}</SelectValue>
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={StaffRole.teacher}>{t("roles.teacher")}</SelectItem>
            <SelectItem value={StaffRole.secretary}>{t("roles.secretary")}</SelectItem>
            <SelectItem value={StaffRole.owner}>{t("roles.owner")}</SelectItem>
          </SelectContent>
        </Select>
      )}
    />
  );
}

function LocaleField({
  control,
  disabled,
}: {
  control: ReturnType<typeof useForm<CreateStaffFormValues>>["control"];
  disabled: boolean;
}) {
  const { t } = useTranslation("common");

  return (
    <Controller
      control={control}
      name="preferredLocale"
      render={({ field }) => (
        <Select value={field.value} onValueChange={field.onChange} disabled={disabled}>
          <SelectTrigger id="staff-locale" className="w-full">
            <SelectValue>
              {(value: CreateStaffFormValues["preferredLocale"]) =>
                t(value === "ar" ? "language.arabic" : "language.english")
              }
            </SelectValue>
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="ar">{t("language.arabic")}</SelectItem>
            <SelectItem value="en">{t("language.english")}</SelectItem>
          </SelectContent>
        </Select>
      )}
    />
  );
}
