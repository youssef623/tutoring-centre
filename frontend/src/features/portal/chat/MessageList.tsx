import { useTranslation } from "react-i18next";
import type { ChatMessage, LocalMessage } from "./useChatMessages";

export function MessageList({
  items,
  optimistic,
  failed,
}: {
  items: ChatMessage[];
  optimistic: LocalMessage | null;
  failed: boolean;
}) {
  const { t, i18n } = useTranslation("portal");
  const chronological = [...new Map(items.map((item) => [item.id, item])).values()].reverse();
  return (
    <ol aria-label={t("chat")} aria-live="polite" className="space-y-5 p-5 sm:p-7">
      {chronological.map((item) => (
        <li
          key={item.id}
          className={`flex flex-col ${item.direction === "inbound" ? "items-end" : "items-start"}`}
        >
          <p
            dir="auto"
            className={`max-w-[85%] whitespace-pre-wrap break-words rounded-xl px-4 py-3 text-sm leading-relaxed ${item.direction === "inbound" ? "bg-primary text-primary-foreground" : "bg-muted text-foreground"}`}
          >
            {item.text}
          </p>
          <time dateTime={item.createdAt} className="mt-1.5 text-xs text-muted-foreground">
            {new Date(item.createdAt).toLocaleTimeString(i18n.language, {
              hour: "2-digit",
              minute: "2-digit",
            })}
          </time>
          {item.status && (
            <span className="mt-1 text-xs text-muted-foreground">
              {t(`chatStatus.${item.status}`)}
            </span>
          )}
        </li>
      ))}
      {optimistic && (
        <li className="flex flex-col items-end" aria-busy={!failed}>
          <p
            dir="auto"
            className="max-w-[85%] whitespace-pre-wrap break-words rounded-xl bg-primary/70 px-4 py-3 text-sm leading-relaxed text-primary-foreground"
          >
            {optimistic.text}
          </p>
          <span className="mt-1.5 text-xs text-muted-foreground">
            {t(failed ? "chatSendFailed" : "chatSending")}
          </span>
        </li>
      )}
    </ol>
  );
}
