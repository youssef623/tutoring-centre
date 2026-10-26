import { keepPreviousData } from "@tanstack/react-query";
import { Check } from "lucide-react";
import { useId, useState, type KeyboardEvent } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListStudents, type StudentListItemDto } from "@/api/generated/tutoring-centre";
import { SearchInput } from "@/components/shared/SearchInput";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "cn";

const PickerPageSize = 10;

/**
 * A picker built from the existing students list query: a small page and the debounced search term, so the
 * server's normalized Arabic search works here with no new endpoint. The search lives in local state, not the
 * URL. Students already on the roster are shown but marked and cannot be picked. Keyboard: Tab from the search
 * box into the list, arrows and Home/End to move, Enter or Space to pick.
 */
export function StudentPicker({
  selectedId,
  onSelect,
  unavailableIds,
}: {
  selectedId: string | null;
  onSelect: (student: StudentListItemDto) => void;
  unavailableIds: ReadonlySet<string>;
}) {
  const { t } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  const listId = useId();
  const [search, setSearch] = useState("");
  const [activeIndex, setActiveIndex] = useState(0);
  const { data, error, isPending, refetch } = useListStudents(
    { search: search === "" ? undefined : search, pageSize: PickerPageSize },
    { query: { placeholderData: keepPreviousData } },
  );
  const items = data?.items ?? [];
  const active = items.at(Math.min(activeIndex, items.length - 1));

  const pick = (student: StudentListItemDto | undefined) => {
    if (student !== undefined && !unavailableIds.has(student.id)) {
      onSelect(student);
    }
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLUListElement>) => {
    const last = items.length - 1;
    const moves = new Map<string, number>([
      ["ArrowDown", Math.min(activeIndex + 1, last)],
      ["ArrowUp", Math.max(activeIndex - 1, 0)],
      ["Home", 0],
      ["End", last],
    ]);
    const next = moves.get(event.key);

    if (next !== undefined) {
      event.preventDefault();
      setActiveIndex(next);
      const option = items.at(next);
      const element = option === undefined ? null : document.getElementById(`${listId}-${option.id}`);
      // jsdom has no scrollIntoView; browsers do.
      if (element !== null && "scrollIntoView" in element) {
        element.scrollIntoView({ block: "nearest" });
      }
    } else if (event.key === "Enter" || event.key === " ") {
      event.preventDefault();
      pick(active);
    }
  };

  return (
    <div className="space-y-2">
      <SearchInput
        value={search}
        onChange={(next) => {
          setSearch(next);
          setActiveIndex(0);
        }}
        label={t("picker.searchLabel")}
      />

      {isPending ? (
        <div role="status" aria-label={t("picker.loadingLabel")} className="space-y-1.5">
          <Skeleton className="h-12 w-full" />
          <Skeleton className="h-12 w-full" />
          <Skeleton className="h-12 w-full" />
        </div>
      ) : error ? (
        <div role="alert" className="flex flex-col items-start gap-2 rounded-lg border border-destructive/40 p-3 text-sm">
          <p>{messageFor(asApiError(error))}</p>
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() => {
              void refetch();
            }}
          >
            {tCommon("retry")}
          </Button>
        </div>
      ) : items.length === 0 ? (
        <p className="rounded-lg border border-dashed border-border px-3 py-6 text-center text-sm text-muted-foreground">
          {t("picker.empty")}
        </p>
      ) : (
        <ul
          role="listbox"
          aria-label={t("picker.listLabel")}
          aria-activedescendant={active === undefined ? undefined : `${listId}-${active.id}`}
          tabIndex={0}
          onKeyDown={handleKeyDown}
          className="max-h-72 divide-y divide-border overflow-y-auto rounded-lg border border-border outline-none focus-visible:ring-3 focus-visible:ring-ring/50"
        >
          {items.map((student, index) => {
            const isUnavailable = unavailableIds.has(student.id);
            const isSelected = student.id === selectedId;
            const details = [student.schoolYear, student.primaryParentName].filter((part) => part !== null);

            return (
              <li
                key={student.id}
                id={`${listId}-${student.id}`}
                role="option"
                aria-selected={isSelected}
                aria-disabled={isUnavailable}
                onClick={() => {
                  setActiveIndex(index);
                  pick(student);
                }}
                className={cn(
                  "flex items-center gap-3 px-3 py-2 text-sm",
                  isUnavailable ? "cursor-not-allowed text-muted-foreground" : "cursor-pointer hover:bg-muted/60",
                  index === activeIndex && "bg-muted",
                  isSelected && "bg-primary/10",
                )}
              >
                <span className="min-w-0 flex-1">
                  <span className="block truncate font-medium">
                    <bdi>{student.fullName}</bdi>
                  </span>
                  {details.length > 0 && (
                    <span className="block truncate text-xs text-muted-foreground">
                      <bdi>{details.join(" · ")}</bdi>
                    </span>
                  )}
                </span>
                {isUnavailable && <span className="shrink-0 text-xs">{t("picker.alreadyInGroup")}</span>}
                {isSelected && <Check className="size-4 shrink-0 text-primary" aria-hidden="true" />}
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
}
