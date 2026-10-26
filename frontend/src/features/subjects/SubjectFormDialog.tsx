import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getListSubjectsQueryKey,
  useCreateSubject,
  useRenameSubject,
  type SubjectDto,
} from "@/api/generated/tutoring-centre";
import { subjectFormSchema, type SubjectFormValues } from "@/features/subjects/subjectSchema";
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

export type SubjectDialogState = { mode: "create" } | { mode: "rename"; subject: SubjectDto } | null;

const KnownFields = ["name"] as const;

export function SubjectFormDialog({
  state,
  onOpenChange,
}: {
  state: SubjectDialogState;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("subjects");
  const queryClient = useQueryClient();
  const createMutation = useCreateSubject();
  const renameMutation = useRenameSubject();

  const {
    register,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isSubmitting, isDirty },
  } = useForm<SubjectFormValues>({ resolver: zodResolver(subjectFormSchema), defaultValues: { name: "" } });

  const open = state !== null;
  const mode = state?.mode;

  useEffect(() => {
    if (state !== null) {
      reset({ name: state.mode === "rename" ? state.subject.name : "" });
    }
  }, [state, reset]);

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });

  const onSubmit = handleSubmit(async (values) => {
    if (state === null) {
      return;
    }

    try {
      if (state.mode === "create") {
        await createMutation.mutateAsync({ data: { name: values.name } });
        await invalidate();
        toast.success(t("toasts.created"));
        onOpenChange(false);
      } else {
        await renameMutation.mutateAsync({
          id: state.subject.id,
          data: { name: values.name, version: state.subject.version },
        });
        await invalidate();
        toast.success(t("toasts.renamed"));
        onOpenChange(false);
      }
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);

      if (state.mode === "rename" && (apiError.code === "concurrency.stale" || apiError.code === "subject.not_found")) {
        await invalidate();
        onOpenChange(false);
      }
    }
  });

  const isRenameUnchanged = mode === "rename" && !isDirty;

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
            <DialogTitle>{t(mode === "rename" ? "dialogs.renameTitle" : "dialogs.createTitle")}</DialogTitle>
            <DialogDescription className="sr-only">
              {t(mode === "rename" ? "dialogs.renameTitle" : "dialogs.createTitle")}
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-1.5 py-2">
            <Label htmlFor="subject-name">{t("form.nameLabel")}</Label>
            <Input
              id="subject-name"
              autoFocus
              disabled={isSubmitting}
              placeholder={t("form.namePlaceholder")}
              aria-invalid={errors.name !== undefined}
              {...register("name")}
            />
            {errors.name !== undefined && (
              <p className="text-sm text-destructive">
                {errors.name.type === "server" ? errors.name.message : t(errors.name.message ?? "")}
              </p>
            )}
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
            <Button type="submit" disabled={isSubmitting || isRenameUnchanged}>
              {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
              {t("form.save")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
