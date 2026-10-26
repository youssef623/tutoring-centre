import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { Controller, useForm, type FieldError } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { applyTranslatedFieldErrors } from "@/api/fieldErrors";
import type { GroupDto } from "@/api/generated/tutoring-centre";
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
import { addDays, WeekdaysFromSaturday } from "@/features/schedule/week";
import { isDoubleBooking, scheduleMessageFor } from "@/features/schedule/scheduleMessages";
import {
  DurationOptions,
  Hours12,
  MinuteOptions,
  RoomMaxLength,
  scheduleRuleSchema,
  toAddScheduleRuleRequest,
  type ScheduleRuleFormValues,
} from "@/features/schedule/scheduleSchemas";
import { useDurationLabel } from "@/features/schedule/useDurationLabel";
import { useScheduleMutations } from "@/features/schedule/useScheduleMutations";

/** Server field names the form shows under its own inputs; the start time's error goes under the time group. */
type FormField = keyof ScheduleRuleFormValues;
const FieldForServerField: Partial<Record<string, FormField>> = {
  dayOfWeek: "dayOfWeek",
  startTime: "hour",
  durationMinutes: "durationMinutes",
  room: "room",
  effectiveFrom: "effectiveFrom",
};

/** How far ahead a rule may take effect (the server's ScheduleHorizon.MaxEffectiveFromDaysAhead). */
const MaxEffectiveFromDaysAhead = 366;

/**
 * Adds a weekly session to a group: a weekday (Saturday first), a start time in the centre's local time in
 * 5-minute steps, a length, an optional room and a first date that defaults to today in the centre's zone. It
 * sends exactly the contract's five fields — never a teacher (the group's teacher teaches it), an offset or an
 * instant. A double booking is refused by the database and reported here, specific to teacher or group, with the
 * dialog left open so the time can be changed.
 */
export function AddScheduleRuleDialog({
  group,
  today,
  timeZoneId,
  open,
  onOpenChange,
}: {
  group: GroupDto;
  today: string;
  timeZoneId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("schedule");

  return (
    <Dialog
      open={open}
      onOpenChange={(next) => {
        if (!next) {
          onOpenChange(false);
        }
      }}
    >
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>{t("addDialog.title")}</DialogTitle>
          <DialogDescription>
            <bdi>{t("addDialog.description", { teacher: group.teacherName })}</bdi>
          </DialogDescription>
        </DialogHeader>
        {open && (
          <AddScheduleRuleForm
            group={group}
            today={today}
            timeZoneId={timeZoneId}
            onDone={() => {
              onOpenChange(false);
            }}
          />
        )}
      </DialogContent>
    </Dialog>
  );
}

function AddScheduleRuleForm({
  group,
  today,
  timeZoneId,
  onDone,
}: {
  group: GroupDto;
  today: string;
  timeZoneId: string;
  onDone: () => void;
}) {
  const { t } = useTranslation("schedule");
  const durationLabel = useDurationLabel();
  const { addRule } = useScheduleMutations();
  const [refusal, setRefusal] = useState<string | null>(null);

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<ScheduleRuleFormValues>({
    resolver: zodResolver(scheduleRuleSchema),
    defaultValues: { hour: 4, minute: 0, period: "pm", durationMinutes: 90, room: "", effectiveFrom: today },
  });

  const onSubmit = handleSubmit(async (values) => {
    setRefusal(null);
    const outcome = await addRule(group.id, toAddScheduleRuleRequest(values));

    if (outcome.ok) {
      toast.success(t("toasts.scheduled", { count: outcome.value }));
      onDone();
      return;
    }

    const { error } = outcome;
    if (isDoubleBooking(error) || error.code === "group.archived") {
      setRefusal(scheduleMessageFor(error));
      return;
    }

    if (error.fieldErrors !== undefined) {
      const mapped = Object.fromEntries(
        Object.entries(error.fieldErrors).map(([field, messages]) => [FieldForServerField[field] ?? field, messages]),
      );
      const knownFields = Object.values(FieldForServerField).filter((field) => field !== undefined);
      if (applyTranslatedFieldErrors({ ...error, fieldErrors: mapped }, setError, knownFields)) {
        return;
      }
    }

    toast.error(scheduleMessageFor(error));
    onDone();
  });

  const timeError = errors.hour ?? errors.minute ?? errors.period;

  return (
    <form
      onSubmit={(event) => {
        void onSubmit(event);
      }}
      noValidate
      className="space-y-4"
    >
      <p className="text-xs text-muted-foreground">{t("timeZoneNote", { zone: timeZoneId })}</p>

      <div className="grid gap-4 sm:grid-cols-2">
        <div className="space-y-1.5">
          <Label htmlFor="add-rule-day">{t("addDialog.dayLabel")}</Label>
          <Controller
            control={control}
            name="dayOfWeek"
            render={({ field }) => (
              <Select
                // No day is chosen until the user picks one: the value is undefined at runtime though typed as a weekday.
                // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
                value={field.value ?? null}
                onValueChange={(next: string | null) => {
                  field.onChange(next ?? undefined);
                }}
                disabled={isSubmitting}
              >
                <SelectTrigger
                  id="add-rule-day"
                  className="w-full"
                  aria-invalid={errors.dayOfWeek !== undefined}
                  aria-describedby={errors.dayOfWeek !== undefined ? "add-rule-dayOfWeek-error" : undefined}
                >
                  <SelectValue placeholder={t("addDialog.dayPlaceholder")}>
                    {(selected: string | null) => (selected === null ? null : t(`weekdays.${selected}`))}
                  </SelectValue>
                </SelectTrigger>
                <SelectContent>
                  {WeekdaysFromSaturday.map((day) => (
                    <SelectItem key={day} value={day}>
                      {t(`weekdays.${day}`)}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          <FieldMessage id="add-rule-dayOfWeek-error" error={errors.dayOfWeek} />
        </div>

        <div className="space-y-1.5">
          <Label htmlFor="add-rule-duration">{t("addDialog.durationLabel")}</Label>
          <Controller
            control={control}
            name="durationMinutes"
            render={({ field }) => (
              <Select
                value={String(field.value)}
                onValueChange={(next: string | null) => {
                  field.onChange(next === null ? undefined : Number(next));
                }}
                disabled={isSubmitting}
              >
                <SelectTrigger
                  id="add-rule-duration"
                  className="w-full"
                  aria-invalid={errors.durationMinutes !== undefined}
                  aria-describedby={errors.durationMinutes !== undefined ? "add-rule-durationMinutes-error" : undefined}
                >
                  <SelectValue>{(selected: string | null) => (selected === null ? null : durationLabel(Number(selected)))}</SelectValue>
                </SelectTrigger>
                <SelectContent>
                  {DurationOptions.map((minutes) => (
                    <SelectItem key={minutes} value={String(minutes)}>
                      {durationLabel(minutes)}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          <FieldMessage id="add-rule-durationMinutes-error" error={errors.durationMinutes} />
        </div>
      </div>

      <fieldset className="space-y-1.5" aria-describedby={timeError !== undefined ? "add-rule-time-error" : undefined}>
        <legend className="text-sm font-medium leading-none">{t("addDialog.timeLabel")}</legend>
        {/* Hour, minute and period read left to right in both languages, like every time on the page. */}
        <div dir="ltr" className="flex items-center gap-2 pt-1.5">
          <Controller
            control={control}
            name="hour"
            render={({ field }) => (
              <Select
                value={String(field.value)}
                onValueChange={(next: string | null) => {
                  field.onChange(next === null ? undefined : Number(next));
                }}
                disabled={isSubmitting}
              >
                <SelectTrigger aria-label={t("addDialog.hourLabel")} className="w-20 tabular-nums" aria-invalid={timeError !== undefined}>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {Hours12.map((hour) => (
                    <SelectItem key={hour} value={String(hour)}>
                      {String(hour)}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          <span aria-hidden="true" className="text-muted-foreground">
            :
          </span>
          <Controller
            control={control}
            name="minute"
            render={({ field }) => (
              <Select
                value={String(field.value)}
                onValueChange={(next: string | null) => {
                  field.onChange(next === null ? undefined : Number(next));
                }}
                disabled={isSubmitting}
              >
                <SelectTrigger aria-label={t("addDialog.minuteLabel")} className="w-20 tabular-nums" aria-invalid={timeError !== undefined}>
                  <SelectValue>{(selected: string | null) => (selected === null ? null : selected.padStart(2, "0"))}</SelectValue>
                </SelectTrigger>
                <SelectContent>
                  {MinuteOptions.map((minute) => (
                    <SelectItem key={minute} value={String(minute)}>
                      {String(minute).padStart(2, "0")}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          <Controller
            control={control}
            name="period"
            render={({ field }) => (
              <Select
                value={field.value}
                onValueChange={(next: string | null) => {
                  field.onChange(next ?? undefined);
                }}
                disabled={isSubmitting}
              >
                <SelectTrigger aria-label={t("addDialog.periodLabel")} className="w-20" aria-invalid={timeError !== undefined}>
                  <SelectValue>{(selected: string | null) => (selected === null ? null : t(`addDialog.${selected}`))}</SelectValue>
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="am">{t("addDialog.am")}</SelectItem>
                  <SelectItem value="pm">{t("addDialog.pm")}</SelectItem>
                </SelectContent>
              </Select>
            )}
          />
        </div>
        <FieldMessage id="add-rule-time-error" error={timeError} />
      </fieldset>

      <div className="grid gap-4 sm:grid-cols-2">
        <div className="space-y-1.5">
          <Label htmlFor="add-rule-room">{t("addDialog.roomLabel")}</Label>
          <Input
            id="add-rule-room"
            dir="auto"
            autoComplete="off"
            maxLength={RoomMaxLength}
            disabled={isSubmitting}
            placeholder={t("addDialog.roomPlaceholder")}
            aria-invalid={errors.room !== undefined}
            aria-describedby={errors.room !== undefined ? "add-rule-room-error" : undefined}
            {...register("room")}
          />
          <FieldMessage id="add-rule-room-error" error={errors.room} />
        </div>

        <div className="space-y-1.5">
          <Label htmlFor="add-rule-from">{t("addDialog.startDateLabel")}</Label>
          <Input
            id="add-rule-from"
            type="date"
            dir="ltr"
            className="text-start"
            max={addDays(today, MaxEffectiveFromDaysAhead)}
            disabled={isSubmitting}
            aria-invalid={errors.effectiveFrom !== undefined}
            aria-describedby={errors.effectiveFrom !== undefined ? "add-rule-effectiveFrom-error" : "add-rule-from-hint"}
            {...register("effectiveFrom")}
          />
          {errors.effectiveFrom === undefined ? (
            <p id="add-rule-from-hint" className="text-xs text-muted-foreground">
              {t("addDialog.startDateHint")}
            </p>
          ) : (
            <FieldMessage id="add-rule-effectiveFrom-error" error={errors.effectiveFrom} />
          )}
        </div>
      </div>

      {refusal !== null && (
        <Alert variant="destructive">
          <AlertDescription>{refusal}</AlertDescription>
        </Alert>
      )}

      <DialogFooter>
        <Button type="button" variant="outline" onClick={onDone}>
          {t("form.cancel")}
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting && <Loader2 className="animate-spin" aria-hidden="true" />}
          {t("addDialog.submit")}
        </Button>
      </DialogFooter>
    </form>
  );
}

function FieldMessage({ id, error }: { id: string; error: FieldError | undefined }) {
  const { t } = useTranslation("schedule");
  if (error === undefined) {
    return null;
  }

  return (
    <p id={id} role="alert" className="text-sm text-destructive">
      {error.type === "server" ? error.message : t(error.message ?? "")}
    </p>
  );
}
