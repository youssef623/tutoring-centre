import { Link } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { useListSubjects } from "@/api/generated/tutoring-centre";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const AllValue = "__all";

/**
 * A select backed by the subjects list, offering active subjects only (Day 46). Never free text: the value is
 * a subject ID, and the server still validates it (unknown, foreign or archived → a refusal shown by the
 * caller under this select). With `allLabel`, a first "all" entry maps to null for use as a list filter.
 */
export function SubjectSelect({
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
  const { data, isPending, isError } = useListSubjects({ includeArchived: false });
  const subjects = data?.items ?? [];
  const nameOf = (subjectId: string) => subjects.find((subject) => subject.id === subjectId)?.name ?? "";

  return (
    <Select
      value={value ?? (allLabel === undefined ? null : AllValue)}
      onValueChange={(next: string | null) => {
        onChange(next === null || next === AllValue ? null : next);
      }}
      disabled={disabled || isPending}
    >
      <SelectTrigger id={id} className={className} aria-invalid={invalid} aria-describedby={describedBy}>
        <SelectValue placeholder={t(isPending ? "selects.loading" : "selects.subjectPlaceholder")}>
          {(selected: string | null) =>
            selected === null ? null : selected === AllValue ? allLabel : <span dir="auto">{nameOf(selected)}</span>
          }
        </SelectValue>
      </SelectTrigger>
      <SelectContent>
        {allLabel !== undefined && <SelectItem value={AllValue}>{allLabel}</SelectItem>}
        {subjects.map((subject) => (
          <SelectItem key={subject.id} value={subject.id}>
            <span dir="auto">{subject.name}</span>
          </SelectItem>
        ))}
        {!isPending && subjects.length === 0 && (
          <div className="space-y-1 px-2 py-2 text-sm">
            <p className="text-muted-foreground">{t(isError ? "selects.loadFailed" : "selects.noSubjects")}</p>
            <Link to="/subjects" search={{ archived: false }} className="font-medium text-primary underline-offset-2 hover:underline">
              {t("selects.goToSubjects")}
            </Link>
          </div>
        )}
      </SelectContent>
    </Select>
  );
}
