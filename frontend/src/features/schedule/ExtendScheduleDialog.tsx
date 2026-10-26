import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
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
import { scheduleMessageFor } from "@/features/schedule/scheduleMessages";
import { useScheduleMutations } from "@/features/schedule/useScheduleMutations";
import { addDays, formatCalendarDate, isIsoDate } from "@/features/schedule/week";

/** How far ahead the schedule may be extended (the server's ScheduleHorizon.MaxExtendDaysAhead). */
export const MaxExtendDaysAhead = 92;

/** The suggested date: eight weeks from today. */
const SuggestedDaysAhead = 56;

/**
 * Creates the sessions of every rule in effect up to a date, from today (in the centre's zone) to 92 days ahead.
 * A manual trigger for work a nightly job takes over from Month 4; the button stays as an override. Repeating it
 * creates nothing new, which the toast reports as "Already up to date". A double booking that only now appears (two
 * rules that reach a shared slot) is named by kind, with advice to review the overlapping weekly sessions.
 */
export function ExtendScheduleDialog({
  today,
  open,
  onOpenChange,
}: {
  today: string;
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
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t("extendDialog.title")}</DialogTitle>
          <DialogDescription>{t("extendDialog.description")}</DialogDescription>
        </DialogHeader>
        {open && (
          <ExtendScheduleForm
            today={today}
            onDone={() => {
              onOpenChange(false);
            }}
          />
        )}
      </DialogContent>
    </Dialog>
  );
}

function ExtendScheduleForm({ today, onDone }: { today: string; onDone: () => void }) {
  const { t, i18n } = useTranslation("schedule");
  const { extend, isExtending } = useScheduleMutations();
  const latest = addDays(today, MaxExtendDaysAhead);
  const [through, setThrough] = useState(addDays(today, SuggestedDaysAhead));
  const [fieldError, setFieldError] = useState<string | null>(null);
  const [refusal, setRefusal] = useState<string | null>(null);
  const latestLabel = formatCalendarDate(latest, i18n.language);

  const handleSubmit = async () => {
    if (!isIsoDate(through) || through < today || through > latest) {
      setFieldError(t("validation.throughRange", { max: latestLabel }));
      return;
    }

    setFieldError(null);
    setRefusal(null);
    const outcome = await extend(through);

    if (outcome.ok) {
      toast.success(t("toasts.extended", { count: outcome.value }));
      onDone();
      return;
    }

    const { error } = outcome;
    if (error.code === "schedule.teacher_double_booked") {
      setRefusal(t("outcomes.extendTeacherDoubleBooked"));
    } else if (error.code === "schedule.group_double_booked") {
      setRefusal(t("outcomes.extendGroupDoubleBooked"));
    } else if (error.code === "schedule.range_invalid") {
      setFieldError(t("validation.throughRange", { max: latestLabel }));
    } else {
      toast.error(scheduleMessageFor(error));
      onDone();
    }
  };

  return (
    <form
      onSubmit={(event) => {
        event.preventDefault();
        void handleSubmit();
      }}
      noValidate
      className="space-y-4"
    >
      <div className="space-y-1.5">
        <Label htmlFor="extend-through">{t("extendDialog.throughLabel")}</Label>
        <Input
          id="extend-through"
          type="date"
          dir="ltr"
          className="text-start"
          min={today}
          max={latest}
          value={through}
          disabled={isExtending}
          aria-invalid={fieldError !== null}
          aria-describedby={fieldError !== null ? "extend-through-error" : "extend-through-hint"}
          onChange={(event) => {
            setThrough(event.target.value);
            setFieldError(null);
          }}
        />
        {fieldError === null ? (
          <p id="extend-through-hint" className="text-xs text-muted-foreground">
            {t("extendDialog.throughHint", { max: latestLabel })}
          </p>
        ) : (
          <p id="extend-through-error" role="alert" className="text-sm text-destructive">
            {fieldError}
          </p>
        )}
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
        <Button type="submit" disabled={isExtending}>
          {isExtending && <Loader2 className="animate-spin" aria-hidden="true" />}
          {t("extendDialog.submit")}
        </Button>
      </DialogFooter>
    </form>
  );
}
