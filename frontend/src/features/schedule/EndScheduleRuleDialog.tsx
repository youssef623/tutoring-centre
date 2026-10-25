import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import type { ScheduleRuleDto } from "@/api/generated/tutoring-centre";
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
import { formatLocalTime } from "@/features/schedule/centreTime";
import { scheduleMessageFor } from "@/features/schedule/scheduleMessages";
import { useScheduleMutations } from "@/features/schedule/useScheduleMutations";
import { formatCalendarDate, isIsoDate } from "@/features/schedule/week";

/**
 * Ends a weekly rule after a last date, which may not be before the day it took effect. Sessions after that date
 * are cancelled by the server, never deleted, and the dialog says so. The body carries exactly `{ lastDate,
 * version }`. A stale version, or a rule someone else ended first, is a toast after which the lists refetch.
 */
export function EndScheduleRuleDialog({
  rule,
  groupId,
  today,
  onClose,
}: {
  rule: ScheduleRuleDto | null;
  groupId: string;
  today: string;
  onClose: () => void;
}) {
  const { t, i18n } = useTranslation("schedule");

  return (
    <Dialog
      open={rule !== null}
      onOpenChange={(next) => {
        if (!next) {
          onClose();
        }
      }}
    >
      <DialogContent>
        {rule !== null && (
          <>
            <DialogHeader>
              <DialogTitle>
                {t("endDialog.title", { day: t(`weekdays.${rule.dayOfWeek}`), time: formatLocalTime(rule.startTime, i18n.language) })}
              </DialogTitle>
              <DialogDescription>{t("endDialog.body")}</DialogDescription>
            </DialogHeader>
            <EndScheduleRuleForm rule={rule} groupId={groupId} today={today} onClose={onClose} />
          </>
        )}
      </DialogContent>
    </Dialog>
  );
}

function EndScheduleRuleForm({
  rule,
  groupId,
  today,
  onClose,
}: {
  rule: ScheduleRuleDto;
  groupId: string;
  today: string;
  onClose: () => void;
}) {
  const { t, i18n } = useTranslation("schedule");
  const { endRule, isEnding } = useScheduleMutations();
  // The earliest date the server accepts is the rule's first date; today is the usual choice once it has started.
  const [lastDate, setLastDate] = useState(rule.effectiveFrom > today ? rule.effectiveFrom : today);
  const [fieldError, setFieldError] = useState<string | null>(null);
  const firstDate = formatCalendarDate(rule.effectiveFrom, i18n.language);

  const handleSubmit = async () => {
    if (!isIsoDate(lastDate)) {
      setFieldError(t("validation.dateRequired"));
      return;
    }

    if (lastDate < rule.effectiveFrom) {
      setFieldError(t("validation.lastDateBeforeStart", { date: firstDate }));
      return;
    }

    setFieldError(null);
    const outcome = await endRule({ ruleId: rule.id, groupId, lastDate, version: rule.version });

    if (outcome.ok) {
      toast.success(t("toasts.ended"));
      onClose();
    } else if (outcome.error.code === "schedule.rule_end_invalid") {
      setFieldError(scheduleMessageFor(outcome.error));
    } else {
      toast.error(scheduleMessageFor(outcome.error));
      onClose();
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
        <Label htmlFor="end-rule-last-date">{t("endDialog.lastDateLabel")}</Label>
        <Input
          id="end-rule-last-date"
          type="date"
          dir="ltr"
          className="text-start"
          min={rule.effectiveFrom}
          value={lastDate}
          disabled={isEnding}
          aria-invalid={fieldError !== null}
          aria-describedby={fieldError !== null ? "end-rule-last-date-error" : "end-rule-last-date-hint"}
          onChange={(event) => {
            setLastDate(event.target.value);
            setFieldError(null);
          }}
        />
        {fieldError === null ? (
          <p id="end-rule-last-date-hint" className="text-xs text-muted-foreground">
            {t("endDialog.lastDateHint", { date: firstDate })}
          </p>
        ) : (
          <p id="end-rule-last-date-error" role="alert" className="text-sm text-destructive">
            {fieldError}
          </p>
        )}
      </div>

      <DialogFooter>
        <Button type="button" variant="outline" onClick={onClose}>
          {t("form.cancel")}
        </Button>
        <Button type="submit" variant="destructive" disabled={isEnding}>
          {isEnding && <Loader2 className="animate-spin" aria-hidden="true" />}
          {t("endDialog.confirm")}
        </Button>
      </DialogFooter>
    </form>
  );
}
