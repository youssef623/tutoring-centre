import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getGetStudentQueryKey,
  getListStudentsQueryKey,
  useUpdateStudent,
  type StudentDetailDto,
} from "@/api/generated/tutoring-centre";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { updateStudentFormSchema, type UpdateStudentFormValues } from "@/features/students/updateStudentSchema";
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

const KnownFields = ["fullName", "phone", "schoolYear"] as const;

/**
 * Prefilled from the student detail query's own data, not a copy kept in this component's state: `values`
 * stays tied to `student` while the dialog is open, so closing and reopening (or a background refetch) always
 * shows the latest loaded row, never a stale snapshot from the moment the dialog first mounted.
 */
export function StudentEditDialog({
  student,
  open,
  onOpenChange,
}: {
  student: StudentDetailDto;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("students");
  const queryClient = useQueryClient();
  const mutation = useUpdateStudent();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<UpdateStudentFormValues>({
    resolver: zodResolver(updateStudentFormSchema),
    values: open ? { fullName: student.fullName, phone: student.phone ?? "", schoolYear: student.schoolYear ?? "" } : undefined,
  });

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),
      queryClient.invalidateQueries({ queryKey: getListStudentsQueryKey() }),
    ]);

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        id: student.id,
        data: {
          fullName: values.fullName,
          phone: values.phone === "" ? null : values.phone,
          schoolYear: values.schoolYear === "" ? null : values.schoolYear,
          version: student.version,
        },
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

      if (apiError.code === "concurrency.stale" || apiError.code === "student.not_found") {
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
            <DialogDescription className="sr-only">{t("dialogs.editTitle")}</DialogDescription>
          </DialogHeader>
          <div className="space-y-3 py-2">
            <div className="space-y-1.5">
              <Label htmlFor="student-edit-full-name">{t("form.fullNameLabel")}</Label>
              <Input
                id="student-edit-full-name"
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
              <Label htmlFor="student-edit-phone">{t("form.phoneLabel")}</Label>
              <Controller
                control={control}
                name="phone"
                render={({ field }) => (
                  <PhoneInput
                    id="student-edit-phone"
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

            <div className="space-y-1.5">
              <Label htmlFor="student-edit-school-year">{t("form.schoolYearLabel")}</Label>
              <Input
                id="student-edit-school-year"
                dir="auto"
                disabled={isSubmitting}
                aria-invalid={errors.schoolYear !== undefined}
                {...register("schoolYear")}
              />
              {errors.schoolYear !== undefined && (
                <p className="text-sm text-destructive">
                  {errors.schoolYear.type === "server" ? errors.schoolYear.message : t(errors.schoolYear.message ?? "")}
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
