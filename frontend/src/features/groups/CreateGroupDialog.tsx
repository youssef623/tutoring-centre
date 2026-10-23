import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyTranslatedFieldErrors, setCodedFieldError } from "@/api/fieldErrors";
import { showApiError } from "@/api/showApiError";
import {
  getListGroupsQueryKey,
  getListSubjectsQueryKey,
  getListTeachersQueryKey,
  useCreateGroup,
} from "@/api/generated/tutoring-centre";
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
import { createGroupFormSchema, type CreateGroupFormValues } from "@/features/groups/groupSchemas";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { SubjectSelect } from "@/features/subjects/SubjectSelect";
import { TeacherSelect } from "@/features/teachers/TeacherSelect";

const KnownFields = ["name", "subjectId", "teacherId", "capacity"] as const;
const SubjectRefusals = new Set(["group.subject_archived", "subject.not_found"]);
const TeacherRefusals = new Set(["group.teacher_archived", "teacher.not_found"]);

const EmptyValues = { name: "", subjectId: "", teacherId: "", capacity: 20 } satisfies CreateGroupFormValues;

/**
 * Creates a group from exactly four fields. Subject and teacher are selects backed by their own lists (active
 * entries only), never free text; a refusal about either (archived meanwhile, or unknown in this centre) lands
 * under that select and refetches its list so the stale entry disappears. On success the new group's own
 * page opens.
 */
export function CreateGroupDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {
  const { t } = useTranslation("groups");
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const mutation = useCreateGroup();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<CreateGroupFormValues>({
    resolver: zodResolver(createGroupFormSchema),
    values: open ? EmptyValues : undefined,
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      const created = await mutation.mutateAsync({
        data: { name: values.name, subjectId: values.subjectId, teacherId: values.teacherId, capacity: values.capacity },
      });
      await queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() });
      toast.success(t("toasts.created"));
      onOpenChange(false);
      await navigate({ to: "/groups/$groupId", params: { groupId: created.id }, search: { tab: "overview" } });
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (SubjectRefusals.has(apiError.code)) {
        setCodedFieldError(apiError, setError, "subjectId");
        await queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });
        return;
      }

      if (TeacherRefusals.has(apiError.code)) {
        setCodedFieldError(apiError, setError, "teacherId");
        await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });
        return;
      }

      if (applyTranslatedFieldErrors(apiError, setError, KnownFields)) {
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    }
  });

  const errorText = (field: (typeof KnownFields)[number]) => {
    const error = errors[field];
    if (error === undefined) {
      return null;
    }

    return (
      <p id={`create-group-${field}-error`} className="text-sm text-destructive">
        {error.type === "server" ? error.message : t(error.message ?? "")}
      </p>
    );
  };

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
            <DialogTitle>{t("dialogs.createTitle")}</DialogTitle>
            <DialogDescription>{t("dialogs.createDescription")}</DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-1.5">
              <Label htmlFor="create-group-name">{t("form.nameLabel")}</Label>
              <Input
                id="create-group-name"
                dir="auto"
                autoFocus
                autoComplete="off"
                disabled={isSubmitting}
                placeholder={t("form.namePlaceholder")}
                aria-invalid={errors.name !== undefined}
                aria-describedby={errors.name !== undefined ? "create-group-name-error" : undefined}
                {...register("name")}
              />
              {errorText("name")}
            </div>

            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-1.5">
                <Label htmlFor="create-group-subject">{t("form.subjectLabel")}</Label>
                <Controller
                  control={control}
                  name="subjectId"
                  render={({ field }) => (
                    <SubjectSelect
                      id="create-group-subject"
                      value={field.value === "" ? null : field.value}
                      onChange={(next) => {
                        field.onChange(next ?? "");
                      }}
                      disabled={isSubmitting}
                      invalid={errors.subjectId !== undefined}
                      describedBy={errors.subjectId !== undefined ? "create-group-subjectId-error" : undefined}
                    />
                  )}
                />
                {errorText("subjectId")}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="create-group-teacher">{t("form.teacherLabel")}</Label>
                <Controller
                  control={control}
                  name="teacherId"
                  render={({ field }) => (
                    <TeacherSelect
                      id="create-group-teacher"
                      value={field.value === "" ? null : field.value}
                      onChange={(next) => {
                        field.onChange(next ?? "");
                      }}
                      disabled={isSubmitting}
                      invalid={errors.teacherId !== undefined}
                      describedBy={errors.teacherId !== undefined ? "create-group-teacherId-error" : undefined}
                    />
                  )}
                />
                {errorText("teacherId")}
              </div>
            </div>

            <div className="space-y-1.5">
              <Label htmlFor="create-group-capacity">{t("form.capacityLabel")}</Label>
              <Input
                id="create-group-capacity"
                type="number"
                inputMode="numeric"
                dir="ltr"
                min={1}
                max={200}
                step={1}
                className="w-32 text-start"
                disabled={isSubmitting}
                aria-invalid={errors.capacity !== undefined}
                aria-describedby={errors.capacity !== undefined ? "create-group-capacity-error" : "create-group-capacity-hint"}
                {...register("capacity", { valueAsNumber: true })}
              />
              {errors.capacity === undefined ? (
                <p id="create-group-capacity-hint" className="text-xs text-muted-foreground">
                  {t("form.capacityHint")}
                </p>
              ) : (
                errorText("capacity")
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
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
              {t("actions.create")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
