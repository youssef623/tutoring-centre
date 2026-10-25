import { Link } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { useListTeachers } from "@/api/generated/tutoring-centre";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const AllValue = "__all";

/**
 * A select backed by the teachers list, offering active teachers only (Day 46). Calls `GET /api/teachers`,
 * which needs `teachers.view`: a caller must render this only for a session holding it (behind `Can`), never
 * rely on a 403 to hide it. The server still validates the chosen ID. With `allLabel`, a first "all" entry
 * maps to null for use as a list filter.
 */
export function TeacherSelect({
  id,
  value,
  onChange,
  allLabel,
  invalid = false,
  describedBy,
  disabled = false,
  className = "w-full",
}: {
  id: string;
  value: string | null;
  onChange: (value: string | null) => void;
  allLabel?: string;
  invalid?: boolean;
  describedBy?: string;
  disabled?: boolean;
  className?: string;
}) {
  const { t } = useTranslation("groups");
  const { data, isPending, isError } = useListTeachers({ includeArchived: false });
  const teachers = data?.items ?? [];
  const nameOf = (teacherId: string) => teachers.find((teacher) => teacher.id === teacherId)?.fullName ?? "";

  return (
    <Select
      value={value ?? (allLabel === undefined ? null : AllValue)}
      onValueChange={(next: string | null) => {
        onChange(next === null || next === AllValue ? null : next);
      }}
      disabled={disabled || isPending}
    >
      <SelectTrigger id={id} className={className} aria-invalid={invalid} aria-describedby={describedBy}>
        <SelectValue>
          {(selected: string | null) =>
            // The render function replaces Base UI's placeholder entirely, so "nothing chosen" renders the hint itself.
            selected === null ? (
              <span className="text-muted-foreground">{t(isPending ? "selects.loading" : "selects.teacherPlaceholder")}</span>
            ) : selected === AllValue ? (
              allLabel
            ) : (
              <span dir="auto">{nameOf(selected)}</span>
            )
          }
        </SelectValue>
      </SelectTrigger>
      <SelectContent>
        {allLabel !== undefined && <SelectItem value={AllValue}>{allLabel}</SelectItem>}
        {teachers.map((teacher) => (
          <SelectItem key={teacher.id} value={teacher.id}>
            <span dir="auto">{teacher.fullName}</span>
          </SelectItem>
        ))}
        {!isPending && teachers.length === 0 && (
          <div className="space-y-1 px-2 py-2 text-sm">
            <p className="text-muted-foreground">{t(isError ? "selects.loadFailed" : "selects.noTeachers")}</p>
            <Link to="/teachers" className="font-medium text-primary underline-offset-2 hover:underline">
              {t("selects.goToTeachers")}
            </Link>
          </div>
        )}
      </SelectContent>
    </Select>
  );
}
