import { useTranslation } from "react-i18next";
import type { GroupDto } from "@/api/generated/tutoring-centre";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { SeatMeter } from "@/features/enrollments/SeatMeter";

/**
 * A radio list of groups with their teacher and seats, used to choose a transfer target or a group to enroll in.
 * Full groups, and groups the caller marks unavailable, are shown but disabled. The seat numbers are the server's
 * and only advisory: the request is still re-checked, and a 409 is shown by the caller.
 */
export function GroupChoiceList({
  groups,
  value,
  onChange,
  label,
  unavailableIds,
}: {
  groups: GroupDto[];
  value: string | null;
  onChange: (groupId: string) => void;
  label: string;
  unavailableIds?: ReadonlySet<string>;
}) {
  const { t } = useTranslation("enrollments");

  return (
    <RadioGroup
      aria-label={label}
      value={value}
      onValueChange={(next: string) => {
        onChange(next);
      }}
      className="max-h-80 overflow-y-auto"
    >
      {groups.map((group) => {
        const isFull = Number(group.enrolledCount) >= Number(group.capacity);
        const isUnavailable = unavailableIds?.has(group.id) ?? false;
        const disabled = isFull || isUnavailable;
        const labelId = `group-choice-${group.id}`;

        return (
          <label
            key={group.id}
            className={
              disabled
                ? "flex cursor-not-allowed items-center gap-3 rounded-lg border border-border p-3 text-muted-foreground"
                : "flex cursor-pointer items-center gap-3 rounded-lg border border-border p-3 hover:bg-muted/60 has-data-checked:border-primary has-data-checked:bg-primary/5"
            }
          >
            <RadioGroupItem value={group.id} disabled={disabled} aria-labelledby={labelId} />
            <span id={labelId} className="min-w-0 flex-1">
              <span className="block truncate font-medium">
                <bdi>{group.name}</bdi>
              </span>
              <span className="block truncate text-xs text-muted-foreground">
                <bdi>{group.teacherName}</bdi>
              </span>
              {isUnavailable && <span className="block text-xs">{t("groupPicker.alreadyEnrolled")}</span>}
            </span>
            <SeatMeter enrolled={Number(group.enrolledCount)} capacity={Number(group.capacity)} />
          </label>
        );
      })}
    </RadioGroup>
  );
}
