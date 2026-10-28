import { Radio } from "@base-ui/react/radio";
import { RadioGroup } from "@base-ui/react/radio-group";
import { CircleCheck, CircleX, Clock, FileCheck, type LucideIcon } from "lucide-react";
import { useTranslation } from "react-i18next";
import { AttendanceStatus } from "@/api/generated/tutoring-centre";
import { cn } from "cn";
import { AttendanceStatusOrder, isAttendanceStatus } from "./attendanceStatus";

const statusStyle: Record<AttendanceStatus, { icon: LucideIcon; selected: string }> = {
  present: { icon: CircleCheck, selected: "data-checked:border-primary data-checked:bg-primary data-checked:text-primary-foreground" },
  late: { icon: Clock, selected: "data-checked:border-warning data-checked:bg-warning/20 data-checked:text-warning-foreground dark:data-checked:text-warning" },
  absent: { icon: CircleX, selected: "data-checked:border-destructive data-checked:bg-destructive/10 data-checked:text-destructive" },
  excused: { icon: FileCheck, selected: "data-checked:border-foreground/40 data-checked:bg-secondary data-checked:text-secondary-foreground" },
};

/**
 * The four statuses as one radio group, each choice a large target (a phone is the teacher's device) with an
 * icon and its name, so the selection never rests on colour alone. The choices run in reading order, so the
 * group reverses by itself in Arabic, and the arrow keys move the selection as in any radio group. `value` is
 * undefined for a student with no record and no tap yet: nothing is selected.
 */
export function StatusToggle({
  studentName,
  value,
  disabled,
  onChange,
}: {
  studentName: string;
  value: AttendanceStatus | undefined;
  disabled: boolean;
  onChange: (status: AttendanceStatus) => void;
}) {
  const { t } = useTranslation("attendance");

  return (
    <RadioGroup
      aria-label={t("roster.toggleLabel", { name: studentName })}
      value={value ?? null}
      disabled={disabled}
      onValueChange={(next) => {
        if (typeof next === "string" && isAttendanceStatus(next)) {
          onChange(next);
        }
      }}
      className="grid grid-cols-4 gap-1.5"
    >
      {AttendanceStatusOrder.map((status) => {
        const { icon: Icon, selected } = statusStyle[status];

        return (
          <Radio.Root
            key={status}
            value={status}
            className={cn(
              "flex min-h-12 cursor-pointer flex-col items-center justify-center gap-0.5 rounded-md border border-input bg-background px-1 py-1.5 text-center text-xs font-medium outline-none transition-colors hover:bg-muted focus-visible:ring-3 focus-visible:ring-ring/50 data-disabled:cursor-not-allowed data-disabled:opacity-50 sm:flex-row sm:gap-1.5 sm:text-sm",
              selected,
            )}
          >
            <Icon className="size-4 shrink-0" aria-hidden="true" />
            <span>{t(`status.${status}`)}</span>
          </Radio.Root>
        );
      })}
    </RadioGroup>
  );
}
