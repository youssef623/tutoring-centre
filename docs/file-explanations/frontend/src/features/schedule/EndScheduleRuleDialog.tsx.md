# `frontend/src/features/schedule/EndScheduleRuleDialog.tsx`

## Purpose

Source comment (intent): Ends a weekly rule after a last date, which may not be before the day it took effect. Sessions after that date are cancelled by the server, never deleted, and the dialog says so. The body carries exactly `{ lastDate, version }`. A stale version, or a rule someone else ended first, is a toast after which the lists refetch.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 157 lines and 4,957 bytes. Source hash: `f10c185f5dbfd9749ef8efcb88b9c197f5d051398e3dddf3ac8f4198ba9b8f3b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Loader2 } from "lucide-react";`
- Line 2: `import { useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { toast } from "sonner";`
- Line 5: `import type { ScheduleRuleDto } from "@/api/generated/tutoring-centre";`
- Line 6: `import { Button } from "@/components/ui/button";`
- Line 7: `import {`
- Line 15: `import { Input } from "@/components/ui/input";`
- Line 16: `import { Label } from "@/components/ui/label";`
- Line 17: `import { formatLocalTime } from "@/features/schedule/centreTime";`
- Line 18: `import { scheduleMessageFor } from "@/features/schedule/scheduleMessages";`
- Line 19: `import { useScheduleMutations } from "@/features/schedule/useScheduleMutations";`
- Line 20: `import { formatCalendarDate, isIsoDate } from "@/features/schedule/week";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 27](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx#L27): `export function EndScheduleRuleDialog({`
- [Line 66](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx#L66): `function EndScheduleRuleForm({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/GroupScheduleTab.tsx:14](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

The excerpt stops at line 90; use the original file for the remaining 67 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
