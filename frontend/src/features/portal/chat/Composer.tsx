import { useState } from "react";
import { Send } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";

export function Composer({
  disabled,
  onSend,
}: {
  disabled: boolean;
  onSend: (text: string) => void;
}) {
  const { t } = useTranslation("portal");
  const [draft, setDraft] = useState("");
  const length = Array.from(draft).length;
  const submit = () => {
    if (disabled || !draft.trim() || length > 1000) return;
    onSend(draft.trim());
    setDraft("");
  };
  return (
    <form
      className="space-y-2 border-t p-5 sm:p-7"
      onSubmit={(event) => {
        event.preventDefault();
        submit();
      }}
    >
      <label htmlFor="chat-message" className="text-sm text-muted-foreground">
        {t("chatMessage")}
      </label>
      <textarea
        id="chat-message"
        dir="auto"
        value={draft}
        disabled={disabled}
        rows={3}
        aria-describedby="chat-length"
        className="w-full resize-y rounded-xl border bg-background px-3 py-3 text-base outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:opacity-60"
        onChange={(event) => {
          setDraft(event.target.value);
        }}
        onKeyDown={(event) => {
          if (event.key === "Enter" && !event.shiftKey && !event.nativeEvent.isComposing) {
            event.preventDefault();
            submit();
          }
        }}
      />
      <div className="flex items-center justify-between gap-3">
        <span
          id="chat-length"
          className={`text-xs ${length > 1000 ? "text-destructive" : "text-muted-foreground"}`}
        >
          {length} / 1000
        </span>
        <Button type="submit" disabled={disabled || !draft.trim() || length > 1000}>
          <Send className="size-4" aria-hidden="true" />
          {t("chatSend")}
        </Button>
      </div>
    </form>
  );
}
