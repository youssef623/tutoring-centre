import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { Button } from "@/components/ui/button";
import { Composer } from "./Composer";
import { MessageList } from "./MessageList";
import { useChatMessages } from "./useChatMessages";

export function ChatPage() {
  const { t } = useTranslation("portal");
  const chat = useChatMessages();
  return (
    <section
      className="mx-auto flex min-h-[65dvh] w-full max-w-3xl flex-col rounded-xl border bg-background text-start"
      aria-labelledby="chat-title"
    >
      <h1 id="chat-title" className="px-5 pt-6 text-3xl font-semibold tracking-tight sm:px-7">
        {t("chat")}
      </h1>
      <div className="min-h-48 flex-1">
        {chat.isPending && (
          <p role="status" className="p-7 text-sm text-muted-foreground">
            {t("chatLoading")}
          </p>
        )}
        {chat.error && (
          <div role="alert" className="p-5">
            <p>{t("chatLoadFailed")}</p>
            <Button
              variant="outline"
              onClick={() => {
                void chat.refetch();
              }}
            >
              {t("retry")}
            </Button>
          </div>
        )}
        {!chat.isPending && !chat.error && !chat.items.length && !chat.optimistic && (
          <p className="p-7 text-muted-foreground">{t("chatEmpty")}</p>
        )}
        {chat.hasNextPage && (
          <Button
            variant="ghost"
            disabled={chat.isFetchingNextPage}
            className="mx-5 mt-4"
            onClick={() => {
              void chat.fetchNextPage();
            }}
          >
            {t("chatOlder")}
          </Button>
        )}
        <MessageList items={chat.items} optimistic={chat.optimistic} failed={!!chat.sendError} />
      </div>
      {chat.sendError && (
        <div
          role="alert"
          className="flex items-center justify-between gap-3 px-5 pb-4 text-sm text-destructive"
        >
          <p>
            {t(asApiError(chat.sendError).status === 429 ? "chatRateLimited" : "chatSendFailed")}
          </p>
          <Button variant="outline" disabled={chat.sending} onClick={chat.retrySend}>
            {t("retry")}
          </Button>
        </div>
      )}
      <Composer disabled={chat.sending || !!chat.sendError} onSend={chat.send} />
    </section>
  );
}
