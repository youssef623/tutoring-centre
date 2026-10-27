# `frontend/src/features/schedule/ExtendScheduleDialog.tsx`

## Purpose

Source comment (intent): How far ahead the schedule may be extended (the server's ScheduleHorizon.MaxExtendDaysAhead).

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 164 lines and 5,314 bytes. Source hash: `87526ecdd5e42a6ab41a7bf64150ebace79d77e911415de9332876a1c93dc8a9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Loader2 } from "lucide-react";`
- Line 2: `import { useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { toast } from "sonner";`
- Line 5: `import { Alert, AlertDescription } from "@/components/ui/alert";`
- Line 6: `import { Button } from "@/components/ui/button";`
- Line 7: `import {`
- Line 15: `import { Input } from "@/components/ui/input";`
- Line 16: `import { Label } from "@/components/ui/label";`
- Line 17: `import { scheduleMessageFor } from "@/features/schedule/scheduleMessages";`
- Line 18: `import { useScheduleMutations } from "@/features/schedule/useScheduleMutations";`
- Line 19: `import { addDays, formatCalendarDate, isIsoDate } from "@/features/schedule/week";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 22](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L22): `export const MaxExtendDaysAhead = 92;`
- [Line 33](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L33): `export function ExtendScheduleDialog({`
- [Line 71](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L71): `function ExtendScheduleForm({ today, onDone }: { today: string; onDone: () => void }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/ExtendScheduleDialog.test.tsx:16](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx#L16)
- [frontend/src/features/schedule/TimetablePage.tsx:17](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L17)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:31](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L31)
- [src/TutoringCentre.Application/Academics/Scheduling/ScheduleHorizon.cs:13](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/ScheduleHorizon.cs#L13)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

The excerpt stops at line 90; use the original file for the remaining 74 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
