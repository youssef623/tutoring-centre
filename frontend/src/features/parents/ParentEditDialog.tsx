import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import { getGetParentQueryKey, useUpdateParent, type ParentDetailDto } from "@/api/generated/tutoring-centre";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { updateParentFormSchema, type UpdateParentFormValues } from "@/features/parents/updateParentSchema";
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
import { PhoneInput } from "@/components/shared/PhoneInput";

const KnownFields = ["fullName", "phone"] as const;

/**
 * A changed phone is also the lookup key future registrations search by, and (from Month 4) the WhatsApp
 * identity — this dialog says so in one sentence rather than letting the consequence be a surprise.
 *
 * On success every student detail query is invalidated, not just this parent's — a parent appears on each of
 * their children's pages, and there is no per-child list of "which detail queries are mounted right now" to
 * invalidate individually. Matched by query key prefix (every `useGetStudent` key is `[\`/api/students/${id}\`]`,
 * distinct from the list query's exact `/api/students` key) rather than by enumerating student ids.
 */
export function ParentEditDialog({
  parent,
  open,
  onOpenChange,
}: {
  parent: ParentDetailDto;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("parents");
  const queryClient = useQueryClient();
  const mutation = useUpdateParent();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<UpdateParentFormValues>({
    resolver: zodResolver(updateParentFormSchema),
    values: open ? { fullName: parent.fullName, phone: parent.phone } : undefined,
  });

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parent.id) }),
      queryClient.invalidateQueries({
        predicate: (query) =>
          typeof query.queryKey[0] === "string" && query.queryKey[0].startsWith("/api/students/"),
      }),
    ]);

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        id: parent.id,
        data: { fullName: values.fullName, phone: values.phone, version: parent.version },
      });
      await invalidate();
      toast.success(t("toasts.updated"));
      onOpenChange(false);
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);

      if (apiError.code === "concurrency.stale" || apiError.code === "parent.not_found") {
        await invalidate();
        onOpenChange(false);
      }
    }
  });

  return (
    <Dialog
      open={open}
      onOpenChange={(next) => {
        if (!next) {
          onOpenChange(false);
        }
      }}
    >
      <DialogContent>
        <form
          onSubmit={(event) => {
            void onSubmit(event);
          }}
          noValidate
        >
          <DialogHeader>
            <DialogTitle>{t("dialogs.editTitle")}</DialogTitle>
            <DialogDescription>{t("form.phoneChangeNotice")}</DialogDescription>
          </DialogHeader>
          <div className="space-y-3 py-2">
            <div className="space-y-1.5">
              <Label htmlFor="parent-edit-full-name">{t("form.fullNameLabel")}</Label>
              <Input
                id="parent-edit-full-name"
                dir="auto"
                autoFocus
                disabled={isSubmitting}
                aria-invalid={errors.fullName !== undefined}
                {...register("fullName")}
              />
              {errors.fullName !== undefined && (
                <p className="text-sm text-destructive">
                  {errors.fullName.type === "server" ? errors.fullName.message : t(errors.fullName.message ?? "")}
                </p>
              )}
            </div>

            <div className="space-y-1.5">
              <Label htmlFor="parent-edit-phone">{t("form.phoneLabel")}</Label>
              <Controller
                control={control}
                name="phone"
                render={({ field }) => (
                  <PhoneInput
                    id="parent-edit-phone"
                    value={field.value}
                    disabled={isSubmitting}
                    aria-invalid={errors.phone !== undefined}
                    onChange={field.onChange}
                    onBlur={field.onBlur}
                  />
                )}
              />
              {errors.phone !== undefined && (
                <p className="text-sm text-destructive">
                  {errors.phone.type === "server" ? errors.phone.message : t(errors.phone.message ?? "")}
                </p>
              )}
            </div>
          </div>
          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => {
                onOpenChange(false);
              }}
            >
              {t("form.cancel")}
            </Button>
            <Button type="submit" disabled={isSubmitting || !isDirty}>
              {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
              {t("form.save")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
