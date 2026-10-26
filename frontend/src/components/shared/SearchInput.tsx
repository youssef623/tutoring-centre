import { X } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Input } from "@/components/ui/input";

const DefaultDebounceMs = 300;

/**
 * A search box controlled by a URL search parameter (Day 41): the caller owns `value` (read from the route's
 * search) and `onChange` (navigates with the new term). Typing updates the visible text immediately but only
 * calls `onChange` after `debounceMs` of no further typing, so the server is never asked once per keystroke.
 * Clearing is immediate — a deliberate action, not something to debounce.
 */
export function SearchInput({
  value,
  onChange,
  label,
  debounceMs = DefaultDebounceMs,
}: {
  value: string;
  onChange: (value: string) => void;
  label?: string;
  debounceMs?: number;
}) {
  const { t } = useTranslation("common");
  const [draft, setDraft] = useState(value);
  const [syncedValue, setSyncedValue] = useState(value);
  const onChangeRef = useRef(onChange);
  useEffect(() => {
    onChangeRef.current = onChange;
  }, [onChange]);

  // An external reset (the URL changed from outside this component — a back-navigation, a "clear filters"
  // action elsewhere) must be reflected without waiting for the debounce. Adjusting state while rendering,
  // not in an effect: React re-renders immediately with the adjusted state before the browser paints, so
  // there is no flash of the stale draft the way a `useEffect` sync would produce.
  if (value !== syncedValue) {
    setSyncedValue(value);
    setDraft(value);
  }

  useEffect(() => {
    if (draft === value) {
      return;
    }

    const timeoutId = setTimeout(() => {
      onChangeRef.current(draft);
    }, debounceMs);

    return () => {
      clearTimeout(timeoutId);
    };
    // Only the draft's own changes should restart the timer; `value` and `debounceMs` are read, not watched.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [draft]);

  const handleClear = () => {
    setDraft("");
    onChange("");
  };

  return (
    <div className="relative">
      <Input
        type="search"
        aria-label={label ?? t("search.label")}
        placeholder={t("search.placeholder")}
        value={draft}
        onChange={(event) => {
          setDraft(event.target.value);
        }}
        className={draft === "" ? undefined : "pe-7"}
      />
      {draft !== "" && (
        <button
          type="button"
          aria-label={t("search.clear")}
          onClick={handleClear}
          className="absolute inset-y-0 end-1.5 flex items-center text-muted-foreground hover:text-foreground"
        >
          <X className="size-3.5" aria-hidden="true" />
        </button>
      )}
    </div>
  );
}
