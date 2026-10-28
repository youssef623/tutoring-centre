import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { Loader2, ScrollText } from "lucide-react";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { applyTranslatedFieldErrors } from "@/api/fieldErrors";
import {
  getGetAttendanceQueryKey,
  getGetSessionAttendanceQueryKey,
  getListAttendanceSessionsQueryKey,
  useCorrectAttendance,
  type AttendanceStudentDto,
} from "@/api/generated/tutoring-centre";
import { showApiError } from "@/api/showApiError";
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
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { AttendanceStatusOrder, isAttendanceStatus } from "./attendanceStatus";
import { correctionFormSchema, ReasonMaxLength, type CorrectionFormValues } from "./correctionSchema";

/** Refusals about this correction itself, shown in the dialog so the user can change it and try again. */
const InDialogCodes = new Set(["attendance.correction_too_early", "attendance.correction_unchanged"]);

/**
 * Changes a recorded status after the session has started, with a reason the audit log keeps for good. It is a
 * separate endpoint from marking and is never done through it. `student` is the row as it was when the dialog
 * opened, version included: if someone else changed the record meanwhile the server answers
 * `concurrency.stale`, and the dialog toasts, refetches and closes so the user decides again on fresh data.
 */
export function CorrectionDialog({
  student,
  sessionId,
  onOpenChange,
}: {
  student: AttendanceStudentDto | null;
  sessionId: string;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("attendance");
  const queryClient = useQueryClient();
  const mutation = useCorrectAttendance();
  const [formError, setFormError] = useState<string | null>(null);
  const record = student?.record ?? null;
  const currentStatus = isAttendanceStatus(record?.status) ? record.status : undefined;

  const {
    control,
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<CorrectionFormValues>({
    resolver: zodResolver(correctionFormSchema),
    defaultValues: { reason: "" },
  });
  const reasonLength = (useWatch({ control, name: "reason" }) as string | undefined)?.trim().length ?? 0;

  const refresh = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetSessionAttendanceQueryKey(sessionId) }),
      queryClient.invalidateQueries({ queryKey: getListAttendanceSessionsQueryKey() }),
      queryClient.invalidateQueries({ queryKey: getGetAttendanceQueryKey() }),
    ]);

  const onSubmit = handleSubmit(async (values) => {
    if (record === null) {
      return;
    }

    setFormError(null);
    try {
      await mutation.mutateAsync({
        recordId: record.recordId,
        data: { status: values.status, reason: values.reason, version: record.version },
      });
      await refresh();
      toast.success(t("correction.saved"));
      onOpenChange(false);
    } catch (thrown) {
      const apiError = asApiError(thrown);
      refetchMeOnPermissionDenied(apiError, queryClient);

      if (InDialogCodes.has(apiError.code)) {
        setFormError(messageFor(apiError));
      } else if (!applyTranslatedFieldErrors(apiError, setError, ["reason", "status"] as const)) {
        // A stale version or a vanished record: say so, show fresh data, and let the user decide again.
        showApiError(apiError);
        await refresh();
        onOpenChange(false);
      }
    }
  });

  return (
    <Dialog
      open={student !== null}
      onOpenChange={(next) => {
        if (!next) {
          onOpenChange(false);
        }
      }}
    >
      <DialogContent>
        {student !== null && record !== null && (
          <form
            onSubmit={(event) => {
              event.preventDefault();
              void onSubmit();
            }}
            noValidate
          >
            <DialogHeader>
              <DialogTitle>{t("correction.title")}</DialogTitle>
              <DialogDescription>
                <bdi>{student.studentName}</bdi>
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-5 py-4">
              <p className="text-sm text-muted-foreground">
                {t("correction.current", {
                  status: currentStatus === undefined ? record.status : t(`status.${currentStatus}`),
                  source: t(`source.${record.source === "qr" ? "qr" : "manual"}`),
                })}
              </p>

              <fieldset className="space-y-2">
                <legend className="text-sm font-medium">{t("correction.newStatus")}</legend>
                <Controller
                  control={control}
                  name="status"
                  render={({ field }) => (
                    <RadioGroup
                      // Nothing is chosen until the user picks, though the form type says a status is always set.
                      // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
                      value={field.value ?? null}
                      disabled={isSubmitting}
                      onValueChange={(next) => {
                        if (typeof next === "string" && isAttendanceStatus(next)) {
                          field.onChange(next);
                        }
                      }}
                      aria-invalid={errors.status !== undefined}
                    >
                      {AttendanceStatusOrder.map((status) => (
                        <div key={status} className="flex min-h-9 items-center gap-2">
                          <RadioGroupItem
                            id={`correction-status-${status}`}
                            value={status}
                            disabled={status === currentStatus}
                          />
                          <Label htmlFor={`correction-status-${status}`} className="cursor-pointer">
                            {t(`status.${status}`)}
                            {status === currentStatus && (
                              <span className="text-muted-foreground"> ({t("correction.currentMarker")})</span>
                            )}
                          </Label>
                        </div>
                      ))}
                    </RadioGroup>
                  )}
                />
                {errors.status !== undefined && (
                  <p className="text-sm text-destructive">
                    {errors.status.type === "server" ? errors.status.message : t(errors.status.message ?? "")}
                  </p>
                )}
              </fieldset>

              <div className="space-y-1.5">
                <Label htmlFor="correction-reason">{t("correction.reasonLabel")}</Label>
                <textarea
                  id="correction-reason"
                  dir="auto"
                  rows={3}
                  disabled={isSubmitting}
                  aria-invalid={errors.reason !== undefined}
                  aria-describedby="correction-reason-hint correction-reason-count"
                  className="w-full min-w-0 resize-y rounded-lg border border-input bg-transparent px-2.5 py-2 text-base transition-colors outline-none placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 md:text-sm"
                  {...register("reason")}
                />
                <div className="flex items-start justify-between gap-3 text-sm">
                  <p id="correction-reason-hint" className="flex items-start gap-1.5 text-muted-foreground">
                    <ScrollText className="mt-0.5 size-4 shrink-0" aria-hidden="true" />
                    {t("correction.auditNote")}
                  </p>
                  <p
                    id="correction-reason-count"
                    dir="ltr"
                    className={reasonLength > ReasonMaxLength ? "shrink-0 text-destructive" : "shrink-0 text-muted-foreground"}
                  >
                    {`${String(reasonLength)} / ${String(ReasonMaxLength)}`}
                  </p>
                </div>
                {errors.reason !== undefined && (
                  <p role="alert" className="text-sm text-destructive">
                    {errors.reason.type === "server" ? errors.reason.message : t(errors.reason.message ?? "")}
                  </p>
                )}
              </div>

              {formError !== null && (
                <Alert variant="destructive" role="alert">
                  <AlertDescription>{formError}</AlertDescription>
                </Alert>
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
                {t("correction.cancel")}
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
                {t("correction.submit")}
              </Button>
            </DialogFooter>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}
