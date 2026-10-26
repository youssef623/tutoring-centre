import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getGetParentQueryKey,
  getGetStudentQueryKey,
  useCreateParent,
  useLinkStudentParent,
  ParentRelationship,
  type StudentDetailDto,
} from "@/api/generated/tutoring-centre";
import { ParentLookupField } from "@/components/shared/ParentLookupField";
import { useParentLookup } from "@/components/shared/useParentLookup";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { addParentFormSchema, type AddParentFormValues } from "@/features/students/addParentSchema";
import { Alert, AlertDescription } from "@/components/ui/alert";
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

const KnownFields = ["fullName", "phone"] as const;

/**
 * Found: link the existing parent directly. Not found: this dialog creates the parent, then links the new id —
 * two requests, never presented as atomic (Day 42 rules; only registration's single transaction gets to say
 * "one request"). If the link half fails after the parent now exists, the dialog remembers that id
 * (`createdParentId`) and a retry only repeats the link, never a second create.
 */
export function AddParentDialog({
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
  const lookup = useParentLookup();
  const createMutation = useCreateParent();
  const linkMutation = useLinkStudentParent();
  const [createdParentId, setCreatedParentId] = useState<string | null>(null);
  const [linkFailed, setLinkFailed] = useState(false);

  const {
    register,
    control,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<AddParentFormValues>({
    resolver: zodResolver(addParentFormSchema),
    defaultValues: { phone: "", fullName: "", relationship: ParentRelationship.father },
  });

  const closeAndReset = () => {
    onOpenChange(false);
    reset({ phone: "", fullName: "", relationship: ParentRelationship.father });
    lookup.reset();
    setCreatedParentId(null);
    setLinkFailed(false);
  };

  const invalidate = (parentId: string) =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),
      queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parentId) }),
    ]);

  const linkParent = async (parentId: string, relationship: AddParentFormValues["relationship"]) => {
    try {
      await linkMutation.mutateAsync({ id: student.id, data: { parentId, relationship, version: student.version } });
      await invalidate(parentId);
      toast.success(t("toasts.parentLinked"));
      closeAndReset();
    } catch (thrown) {
      const apiError = asApiError(thrown);
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      setLinkFailed(true);

      if (apiError.code === "concurrency.stale" || apiError.code === "student.not_found") {
        await queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) });
      }
    }
  };

  const onSubmit = handleSubmit(async (values) => {
    if (createdParentId !== null) {
      await linkParent(createdParentId, values.relationship);
      return;
    }

    const outcome = await lookup.ensureLookup(values.phone);
    if (!outcome.ok) {
      return;
    }

    if (outcome.parent !== null) {
      await linkParent(outcome.parent.id, values.relationship);
      return;
    }

    const fullName = values.fullName.trim();
    if (fullName === "") {
      setError("fullName", { type: "manual", message: "validation.parentFullNameRequired" });
      return;
    }

    try {
      const created = await createMutation.mutateAsync({ data: { fullName, phone: values.phone } });
      setCreatedParentId(created.id);
      await linkParent(created.id, values.relationship);
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

  const disabled = isSubmitting || lookup.pending || createMutation.isPending || linkMutation.isPending;
  const parentAlreadyCreated = createdParentId !== null;

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
        <form
          onSubmit={(event) => {
            void onSubmit(event);
          }}
          noValidate
        >
          <DialogHeader>
            <DialogTitle>{t("dialogs.addParentTitle")}</DialogTitle>
            <DialogDescription className="sr-only">{t("dialogs.addParentTitle")}</DialogDescription>
          </DialogHeader>
          <div className="space-y-3 py-2">
            {linkFailed && (
              <Alert>
                <AlertDescription>{t("addParent.linkFailedNotice")}</AlertDescription>
              </Alert>
            )}

            <Controller
              control={control}
              name="phone"
              render={({ field }) => (
                <ParentLookupField
                  id="add-parent-phone"
                  label={t("form.parentPhoneLabel")}
                  value={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  disabled={disabled || parentAlreadyCreated}
                  error={
                    errors.phone === undefined
                      ? undefined
                      : errors.phone.type === "server"
                        ? errors.phone.message
                        : t(errors.phone.message ?? "")
                  }
                  lookup={lookup}
                />
              )}
            />

            <div className="space-y-1.5">
              <Label htmlFor="add-parent-relationship">{t("form.relationshipLabel")}</Label>
              <Controller
                control={control}
                name="relationship"
                render={({ field }) => (
                  <Select value={field.value} onValueChange={field.onChange} disabled={disabled}>
                    <SelectTrigger id="add-parent-relationship" className="w-full">
                      <SelectValue>{(value: AddParentFormValues["relationship"]) => t(`relationship.${value}`)}</SelectValue>
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value={ParentRelationship.father}>{t("relationship.father")}</SelectItem>
                      <SelectItem value={ParentRelationship.mother}>{t("relationship.mother")}</SelectItem>
                      <SelectItem value={ParentRelationship.guardian}>{t("relationship.guardian")}</SelectItem>
                      <SelectItem value={ParentRelationship.other}>{t("relationship.other")}</SelectItem>
                    </SelectContent>
                  </Select>
                )}
              />
            </div>

            {!parentAlreadyCreated && lookup.found === null && (
              <div className="space-y-1.5">
                <Label htmlFor="add-parent-full-name">{t("form.parentFullNameLabel")}</Label>
                <Input
                  id="add-parent-full-name"
                  dir="auto"
                  disabled={disabled}
                  placeholder={t("form.parentFullNamePlaceholder")}
                  aria-invalid={errors.fullName !== undefined}
                  {...register("fullName")}
                />
                {errors.fullName !== undefined && (
                  <p className="text-sm text-destructive">
                    {errors.fullName.type === "server" ? errors.fullName.message : t(errors.fullName.message ?? "")}
                  </p>
                )}
              </div>
            )}

            {lookup.found !== null && !parentAlreadyCreated && (
              <p className="text-sm text-muted-foreground" dir="auto">
                {t("lookup.existingParent", { name: lookup.found.fullName })}
              </p>
            )}
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={closeAndReset}>
              {t("form.cancel")}
            </Button>
            <Button type="submit" disabled={disabled}>
              {disabled && <Loader2 className="animate-spin" aria-hidden="true" />}
              {parentAlreadyCreated ? t("addParent.retryLink") : t("addParent.link")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
