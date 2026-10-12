import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate } from "@tanstack/react-router";
import { Loader2 } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { applyFieldErrors, showApiError } from "@/api/showApiError";
import {
  getListStudentsQueryKey,
  useRegisterStudent,
  ParentRelationship,
  type RegisterStudentRequestParent,
} from "@/api/generated/tutoring-centre";
import { ParentLookupField } from "@/components/shared/ParentLookupField";
import { useParentLookup } from "@/components/shared/useParentLookup";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { registerStudentFormSchema, type RegisterStudentFormValues } from "@/features/students/registerStudentSchema";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { PhoneInput } from "@/components/shared/PhoneInput";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const KnownFields = ["fullName", "phone", "schoolYear", "parent.phone", "parent.fullName"] as const;

/**
 * The form the lookup prevents from registering a sibling's parent twice: the client never decides that a
 * phone is valid or unique, it asks the server (via `lookup`) and renders whichever branch the answer picks.
 * Section order follows the Day 42 contract exactly — student fields, then the parent's phone, relationship,
 * and only then either the existing-parent panel or the new parent's name — because a reviewer checks the
 * screen against that order, not just against the request it finally sends.
 */
export function RegisterStudentForm() {
  const { t } = useTranslation("students");
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const registerMutation = useRegisterStudent();
  const lookup = useParentLookup();

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<RegisterStudentFormValues>({
    resolver: zodResolver(registerStudentFormSchema),
    defaultValues: {
      fullName: "",
      phone: "",
      schoolYear: "",
      relationship: ParentRelationship.father,
      parent: { phone: "", fullName: "" },
    },
  });

  const onSubmit = handleSubmit(async (values) => {
    const outcome = await lookup.ensureLookup(values.parent.phone);
    if (!outcome.ok) {
      return;
    }

    let parent: RegisterStudentRequestParent;
    if (outcome.parent !== null) {
      parent = { id: outcome.parent.id };
    } else {
      const parentFullName = values.parent.fullName.trim();
      if (parentFullName === "") {
        setError("parent.fullName", { type: "manual", message: "validation.parentFullNameRequired" });
        return;
      }

      parent = { fullName: parentFullName, phone: values.parent.phone };
    }

    try {
      const result = await registerMutation.mutateAsync({
        data: {
          fullName: values.fullName,
          phone: values.phone === "" ? null : values.phone,
          schoolYear: values.schoolYear === "" ? null : values.schoolYear,
          parent,
          relationship: values.relationship,
        },
      });

      await queryClient.invalidateQueries({ queryKey: getListStudentsQueryKey() });
      toast.success(t("toasts.registered"));
      await navigate({ to: "/students/$studentId", params: { studentId: result.id } });
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (apiError.code === "parent.phone_taken") {
        await lookup.ensureLookup(values.parent.phone, { force: true });
        return;
      }

      if (apiError.fieldErrors !== undefined) {
        applyFieldErrors(apiError, setError, KnownFields);
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    }
  });

  const disabled = isSubmitting || lookup.pending || registerMutation.isPending;

  return (
    <form
      className="max-w-lg space-y-6"
      onSubmit={(event) => {
        void onSubmit(event);
      }}
      noValidate
    >
      <fieldset className="space-y-4">
        <legend className="text-base font-semibold tracking-tight">{t("form.studentSectionTitle")}</legend>

        <div className="space-y-1.5">
          <Label htmlFor="student-full-name">{t("form.fullNameLabel")}</Label>
          <Input
            id="student-full-name"
            dir="auto"
            autoFocus
            disabled={disabled}
            placeholder={t("form.fullNamePlaceholder")}
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
          <Label htmlFor="student-phone">{t("form.phoneLabel")}</Label>
          <Controller
            control={control}
            name="phone"
            render={({ field }) => (
              <PhoneInput
                id="student-phone"
                value={field.value}
                disabled={disabled}
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
          <Label htmlFor="student-school-year">{t("form.schoolYearLabel")}</Label>
          <Input
            id="student-school-year"
            dir="auto"
            disabled={disabled}
            placeholder={t("form.schoolYearPlaceholder")}
            aria-invalid={errors.schoolYear !== undefined}
            {...register("schoolYear")}
          />
          {errors.schoolYear !== undefined && (
            <p className="text-sm text-destructive">
              {errors.schoolYear.type === "server" ? errors.schoolYear.message : t(errors.schoolYear.message ?? "")}
            </p>
          )}
        </div>
      </fieldset>

      <fieldset className="space-y-4">
        <legend className="text-base font-semibold tracking-tight">{t("form.parentSectionTitle")}</legend>

        <Controller
          control={control}
          name="parent.phone"
          render={({ field }) => (
            <ParentLookupField
              id="parent-phone"
              label={t("form.parentPhoneLabel")}
              value={field.value}
              onChange={field.onChange}
              onBlur={field.onBlur}
              disabled={disabled}
              error={
                errors.parent?.phone === undefined
                  ? undefined
                  : errors.parent.phone.type === "server"
                    ? errors.parent.phone.message
                    : t(errors.parent.phone.message ?? "")
              }
              lookup={lookup}
            />
          )}
        />

        <div className="space-y-1.5">
          <Label htmlFor="parent-relationship">{t("form.relationshipLabel")}</Label>
          <Controller
            control={control}
            name="relationship"
            render={({ field }) => (
              <Select value={field.value} onValueChange={field.onChange} disabled={disabled}>
                <SelectTrigger id="parent-relationship" className="w-full">
                  <SelectValue>{(value: RegisterStudentFormValues["relationship"]) => t(`relationship.${value}`)}</SelectValue>
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

        {lookup.found !== null ? (
          <div className="rounded-lg border border-border bg-muted/50 p-3 text-sm">
            <p dir="auto">{t("lookup.existingParent", { name: lookup.found.fullName })}</p>
            <Button
              type="button"
              variant="link"
              className="h-auto p-0 text-sm"
              onClick={() => {
                lookup.reset();
              }}
            >
              {t("lookup.changeNumber")}
            </Button>
          </div>
        ) : (
          <div className="space-y-1.5">
            <Label htmlFor="parent-full-name">{t("form.parentFullNameLabel")}</Label>
            <Input
              id="parent-full-name"
              dir="auto"
              disabled={disabled}
              placeholder={t("form.parentFullNamePlaceholder")}
              aria-invalid={errors.parent?.fullName !== undefined}
              {...register("parent.fullName")}
            />
            {errors.parent?.fullName !== undefined && (
              <p className="text-sm text-destructive">
                {errors.parent.fullName.type === "server"
                  ? errors.parent.fullName.message
                  : t(errors.parent.fullName.message ?? "")}
              </p>
            )}
          </div>
        )}
      </fieldset>

      <div className="flex items-center gap-3">
        <Button type="submit" disabled={disabled}>
          {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
          {t("form.save")}
        </Button>
        <Button type="button" variant="outline" render={<Link to="/students" />}>
          {t("form.cancel")}
        </Button>
      </div>
    </form>
  );
}
