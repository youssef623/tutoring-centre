import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import type { ReactNode } from "react";
import { Controller, useForm, type FieldError } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import type { ApiError } from "@/api/errors";
import { applyTranslatedFieldErrors, setCodedFieldError } from "@/api/fieldErrors";
import {
  getGetGroupQueryKey,
  getListGroupsQueryKey,
  getListTeachersQueryKey,
  useAssignGroupTeacher,
  useChangeGroupCapacity,
  useRenameGroup,
  type GroupDto,
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
import {
  assignGroupTeacherFormSchema,
  groupCapacityFormSchema,
  renameGroupFormSchema,
  type AssignGroupTeacherFormValues,
  type GroupCapacityFormValues,
  type RenameGroupFormValues,
} from "@/features/groups/groupSchemas";
import { showGroupError } from "@/features/groups/showGroupError";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { TeacherSelect } from "@/features/teachers/TeacherSelect";

/*
 * Three facets, three dialogs, three requests — never one "Edit group" form posting to several endpoints in
 * sequence, where a failure in the second would leave a half-applied edit and a stale version. Each dialog
 * sends the version the page loaded; on `concurrency.stale` it toasts, closes and the page refetches.
 */

interface GroupDialogProps {
  group: GroupDto;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function useGroupMutationOutcome(group: GroupDto, onOpenChange: (open: boolean) => void) {
  const queryClient = useQueryClient();

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetGroupQueryKey(group.id) }),
      queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() }),
    ]);

  const succeed = async (message: string) => {
    await invalidate();
    toast.success(message);
    onOpenChange(false);
  };

  /** Toast for anything not shown in the form; a stale version or a vanished group also closes and refetches. */
  const fail = async (apiError: ApiError) => {
    showGroupError(apiError);
    refetchMeOnPermissionDenied(apiError, queryClient);
    await invalidate();
    onOpenChange(false);
  };

  return { succeed, fail, queryClient };
}

function FieldMessage({
  id,
  error,
  t,
}: {
  id: string;
  error: FieldError | undefined;
  t: (key: string) => string;
}) {
  if (error === undefined) {
    return null;
  }

  return (
    <p id={id} className="text-sm text-destructive">
      {error.type === "server" ? error.message : t(error.message ?? "")}
    </p>
  );
}

function GroupDialogFrame({
  open,
  onOpenChange,
  title,
  description,
  submitting,
  submitDisabled,
  onSubmit,
  children,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  description: string;
  submitting: boolean;
  submitDisabled: boolean;
  onSubmit: () => void;
  children: ReactNode;
}) {
  const { t } = useTranslation("groups");

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
            event.preventDefault();
            onSubmit();
          }}
          noValidate
        >
          <DialogHeader>
            <DialogTitle>{title}</DialogTitle>
            <DialogDescription>{description}</DialogDescription>
          </DialogHeader>
          <div className="space-y-1.5 py-4">{children}</div>
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
            <Button type="submit" disabled={submitting || submitDisabled}>
              {submitting && <Loader2 className="animate-spin" aria-hidden="true" />}
              {t("form.save")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export function RenameGroupDialog({ group, open, onOpenChange }: GroupDialogProps) {
  const { t } = useTranslation("groups");
  const mutation = useRenameGroup();
  const { succeed, fail } = useGroupMutationOutcome(group, onOpenChange);
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<RenameGroupFormValues>({
    resolver: zodResolver(renameGroupFormSchema),
    values: open ? { name: group.name, monthlyFee: group.monthlyFee ?? "0.00" } : undefined,
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        id: group.id,
        data: {
          name: values.name,
          version: group.version,
          ...(values.monthlyFee !== (group.monthlyFee ?? "0.00") && values.monthlyFee !== undefined
            ? { monthlyFee: Number(values.monthlyFee) }
            : {}),
        },
      });
      await succeed(t("toasts.renamed"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      if (!applyTranslatedFieldErrors(apiError, setError, ["name", "monthlyFee"] as const)) {
        await fail(apiError);
      }
    }
  });

  return (
    <GroupDialogFrame
      open={open}
      onOpenChange={onOpenChange}
      title={t("dialogs.renameTitle")}
      description={t("dialogs.renameDescription")}
      submitting={isSubmitting}
      submitDisabled={!isDirty}
      onSubmit={() => {
        void onSubmit();
      }}
    >
      <Label htmlFor="rename-group-name">{t("form.nameLabel")}</Label>
      <Input
        id="rename-group-name"
        dir="auto"
        autoFocus
        autoComplete="off"
        disabled={isSubmitting}
        aria-invalid={errors.name !== undefined}
        aria-describedby={errors.name !== undefined ? "rename-group-name-error" : undefined}
        {...register("name")}
      />
      <FieldMessage id="rename-group-name-error" error={errors.name} t={t} />
      <Label htmlFor="rename-group-monthlyFee">{t("form.monthlyFeeLabel")}</Label>
      <Input
        id="rename-group-monthlyFee"
        type="number"
        inputMode="decimal"
        min="0"
        max="99999999.99"
        step="0.01"
        dir="ltr"
        disabled={isSubmitting}
        {...register("monthlyFee")}
        aria-invalid={errors.monthlyFee !== undefined}
      />
      <p className="text-xs text-muted-foreground">{t("form.monthlyFeeHint")}</p>
      <FieldMessage id="rename-group-monthlyFee-error" error={errors.monthlyFee} t={t} />
    </GroupDialogFrame>
  );
}

export function ChangeGroupCapacityDialog({ group, open, onOpenChange }: GroupDialogProps) {
  const { t } = useTranslation("groups");
  const mutation = useChangeGroupCapacity();
  const { succeed, fail } = useGroupMutationOutcome(group, onOpenChange);
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<GroupCapacityFormValues>({
    resolver: zodResolver(groupCapacityFormSchema),
    values: open ? { capacity: Number(group.capacity) } : undefined,
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        id: group.id,
        data: { capacity: values.capacity, version: group.version },
      });
      await succeed(t("toasts.capacityChanged"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      if (!applyTranslatedFieldErrors(apiError, setError, ["capacity"] as const)) {
        await fail(apiError);
      }
    }
  });

  return (
    <GroupDialogFrame
      open={open}
      onOpenChange={onOpenChange}
      title={t("dialogs.capacityTitle")}
      description={t("dialogs.capacityDescription", {
        enrolled: String(Number(group.enrolledCount)),
      })}
      submitting={isSubmitting}
      submitDisabled={!isDirty}
      onSubmit={() => {
        void onSubmit();
      }}
    >
      <Label htmlFor="change-group-capacity">{t("form.capacityLabel")}</Label>
      <Input
        id="change-group-capacity"
        type="number"
        inputMode="numeric"
        dir="ltr"
        min={1}
        max={200}
        step={1}
        autoFocus
        className="w-32 text-start"
        disabled={isSubmitting}
        aria-invalid={errors.capacity !== undefined}
        aria-describedby={
          errors.capacity !== undefined
            ? "change-group-capacity-error"
            : "change-group-capacity-hint"
        }
        {...register("capacity", { valueAsNumber: true })}
      />
      {errors.capacity === undefined ? (
        <p id="change-group-capacity-hint" className="text-xs text-muted-foreground">
          {t("form.capacityHint")}
        </p>
      ) : (
        <FieldMessage id="change-group-capacity-error" error={errors.capacity} t={t} />
      )}
    </GroupDialogFrame>
  );
}

const TeacherRefusals = new Set(["group.teacher_archived", "teacher.not_found"]);

export function AssignGroupTeacherDialog({ group, open, onOpenChange }: GroupDialogProps) {
  const { t } = useTranslation("groups");
  const mutation = useAssignGroupTeacher();
  const { succeed, fail, queryClient } = useGroupMutationOutcome(group, onOpenChange);
  const {
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<AssignGroupTeacherFormValues>({
    resolver: zodResolver(assignGroupTeacherFormSchema),
    values: open ? { teacherId: group.teacherId } : undefined,
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      await mutation.mutateAsync({
        id: group.id,
        data: { teacherId: values.teacherId, version: group.version },
      });
      await succeed(t("toasts.teacherChanged"));
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (TeacherRefusals.has(apiError.code)) {
        setCodedFieldError(apiError, setError, "teacherId");
        await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });
        return;
      }

      if (!applyTranslatedFieldErrors(apiError, setError, ["teacherId"] as const)) {
        await fail(apiError);
      }
    }
  });

  return (
    <GroupDialogFrame
      open={open}
      onOpenChange={onOpenChange}
      title={t("dialogs.teacherTitle")}
      description={t("dialogs.teacherDescription")}
      submitting={isSubmitting}
      submitDisabled={!isDirty}
      onSubmit={() => {
        void onSubmit();
      }}
    >
      <Label htmlFor="assign-group-teacher">{t("form.teacherLabel")}</Label>
      <Controller
        control={control}
        name="teacherId"
        render={({ field }) => (
          <TeacherSelect
            id="assign-group-teacher"
            value={field.value === "" ? null : field.value}
            onChange={(next) => {
              field.onChange(next ?? "");
            }}
            disabled={isSubmitting}
            invalid={errors.teacherId !== undefined}
            describedBy={errors.teacherId !== undefined ? "assign-group-teacher-error" : undefined}
          />
        )}
      />
      <FieldMessage id="assign-group-teacher-error" error={errors.teacherId} t={t} />
    </GroupDialogFrame>
  );
}
