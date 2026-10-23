import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyTranslatedFieldErrors } from "@/api/fieldErrors";
import { showApiError } from "@/api/showApiError";
import {
  getListTeachersQueryKey,
  useCreateTeacher,
  useUpdateTeacher,
  type TeacherDto,
} from "@/api/generated/tutoring-centre";
import { PhoneInput } from "@/components/shared/PhoneInput";
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
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { teacherFormSchema, type TeacherFormValues } from "@/features/teachers/teacherSchema";

export type TeacherDialogState = { mode: "create" } | { mode: "edit"; teacher: TeacherDto } | null;

const KnownFields = ["fullName", "phone"] as const;

/**
 * Create and edit share one form: full name and an optional phone, nothing else. The account link is never a
 * field here — it is its own dialog and its own request (PUT .../account), so editing details cannot touch it.
 */
export function TeacherFormDialog({
  state,
  onOpenChange,
}: {
  state: TeacherDialogState;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("teachers");
  const queryClient = useQueryClient();
  const createMutation = useCreateTeacher();
  const updateMutation = useUpdateTeacher();
  const open = state !== null;
  const mode = state?.mode;

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<TeacherFormValues>({
    resolver: zodResolver(teacherFormSchema),
    values:
      state === null
        ? undefined
        : state.mode === "edit"
          ? { fullName: state.teacher.fullName, phone: state.teacher.phone ?? "" }
          : { fullName: "", phone: "" },
  });

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });

  const onSubmit = handleSubmit(async (values) => {
    if (state === null) {
      return;
    }

    const phone = values.phone === "" ? null : values.phone;

    try {
      if (state.mode === "create") {
        await createMutation.mutateAsync({ data: { fullName: values.fullName, phone } });
        await invalidate();
        toast.success(t("toasts.created"));
      } else {
        await updateMutation.mutateAsync({
          id: state.teacher.id,
          data: { fullName: values.fullName, phone, version: state.teacher.version },
        });
        await invalidate();
        toast.success(t("toasts.updated"));
      }

      onOpenChange(false);
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (applyTranslatedFieldErrors(apiError, setError, KnownFields)) {
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);

      if (state.mode === "edit") {
        await invalidate();
        onOpenChange(false);
      }
    }
  });

  const title = t(mode === "edit" ? "dialogs.editTitle" : "dialogs.createTitle");

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
            <DialogTitle>{title}</DialogTitle>
            <DialogDescription>{t("dialogs.formDescription")}</DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-1.5">
              <Label htmlFor="teacher-full-name">{t("form.fullNameLabel")}</Label>
              <Input
                id="teacher-full-name"
                dir="auto"
                autoFocus
                autoComplete="off"
                disabled={isSubmitting}
                placeholder={t("form.fullNamePlaceholder")}
                aria-invalid={errors.fullName !== undefined}
                aria-describedby={errors.fullName !== undefined ? "teacher-full-name-error" : undefined}
                {...register("fullName")}
              />
              {errors.fullName !== undefined && (
                <p id="teacher-full-name-error" className="text-sm text-destructive">
                  {errors.fullName.type === "server" ? errors.fullName.message : t(errors.fullName.message ?? "")}
                </p>
              )}
            </div>

            <div className="space-y-1.5">
              <Label htmlFor="teacher-phone">{t("form.phoneLabel")}</Label>
              <Controller
                control={control}
                name="phone"
                render={({ field }) => (
                  <PhoneInput
                    id="teacher-phone"
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
            <Button type="submit" disabled={isSubmitting || (mode === "edit" && !isDirty)}>
              {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
              {t("form.save")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
