# `frontend/src/features/schedule/AddScheduleRuleDialog.tsx`

## Purpose

Source comment (intent): Server field names the form shows under its own inputs; the start time's error goes under the time group.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 395 lines and 14,455 bytes. Source hash: `590fe93424dd7e79daa8f4f7d3cb245df2fad2e968199378a2db3c72f3c4e085`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { zodResolver } from "@hookform/resolvers/zod";`
- Line 2: `import { Loader2 } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { Controller, useForm, type FieldError } from "react-hook-form";`
- Line 5: `import { useTranslation } from "react-i18next";`
- Line 6: `import { toast } from "sonner";`
- Line 7: `import { applyTranslatedFieldErrors } from "@/api/fieldErrors";`
- Line 8: `import type { GroupDto } from "@/api/generated/tutoring-centre";`
- Line 9: `import { Alert, AlertDescription } from "@/components/ui/alert";`
- Line 10: `import { Button } from "@/components/ui/button";`
- Line 11: `import {`
- Line 19: `import { Input } from "@/components/ui/input";`
- Line 20: `import { Label } from "@/components/ui/label";`
- Line 21: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`
- Line 22: `import { addDays, WeekdaysFromSaturday } from "@/features/schedule/week";`
- Line 23: `import { isDoubleBooking, scheduleMessageFor } from "@/features/schedule/scheduleMessages";`
- Line 24: `import {`
- Line 33: `import { useDurationLabel } from "@/features/schedule/useDurationLabel";`
- Line 34: `import { useScheduleMutations } from "@/features/schedule/useScheduleMutations";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 56](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L56): `export function AddScheduleRuleDialog({`
- [Line 102](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L102): `function AddScheduleRuleForm({`
- [Line 384](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L384): `function FieldMessage({ id, error }: { id: string; error: FieldError | undefined }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:36](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L36)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:12](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L12)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

The excerpt stops at line 90; use the original file for the remaining 305 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
