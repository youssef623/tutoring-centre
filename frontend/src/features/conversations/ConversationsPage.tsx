import { RunBlock, type RunItem } from "./RunBlock";
import { useCan } from "@/features/session/useCan";
import { Permissions } from "@/features/session/permissions";
import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { apiFetch } from "@/api/apiFetch";
import { Button } from "@/components/ui/button";
import { MessageDetailsDrawer, type ConversationMessage } from "./MessageDetailsDrawer";

interface Conversation {
  id: string;
  address: string;
  channel?: string;
  lastMessageAt: string;
}
export function ConversationsPage() {
  const { t, i18n } = useTranslation("conversations");
  const canInspect = useCan(Permissions.AssistantView);
  const [selected, setSelected] = useState<string | null>(null);
  const [details, setDetails] = useState<ConversationMessage | null>(null);
  const conversations = useQuery({
    queryKey: ["conversations"],
    queryFn: ({ signal }) => apiFetch<{ items: Conversation[] }>("/api/conversations", { signal }),
  });
  const messages = useInfiniteQuery({
    queryKey: ["conversation-messages", selected, canInspect],
    enabled: !!selected,
    initialPageParam: null as string | null,
    queryFn: ({ pageParam, signal }) =>
      apiFetch<{ items: (ConversationMessage & RunItem)[]; nextCursor: string | null }>(
        `/api/conversations/${selected ?? ""}/${canInspect ? "timeline" : "messages"}${pageParam ? `?cursor=${encodeURIComponent(pageParam)}` : ""}`,
        { signal },
      ),
    getNextPageParam: (page) => page.nextCursor,
  });
  const items =
    messages.data?.pages.flatMap((page) =>
      page.items.map((item) => ({
        ...item,
        createdAt: item.at ?? item.createdAt,
        direction: item.kind === "inbound" || item.kind === "outbound" ? item.kind : item.direction,
      })),
    ) ?? [];
  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-semibold tracking-tight">{t("title")}</h1>
      <div className="grid min-h-[65dvh] overflow-hidden rounded-xl border md:grid-cols-[16rem_1fr]">
        <aside className="border-b md:border-e md:border-b-0" aria-label={t("title")}>
          {conversations.isPending && (
            <p role="status" className="p-5">
              {t("loading")}
            </p>
          )}
          {conversations.error && (
            <div className="p-5" role="alert">
              {t("loadFailed")}
              <Button
                onClick={() => {
                  void conversations.refetch();
                }}
              >
                {t("retry")}
              </Button>
            </div>
          )}
          {conversations.data?.items.length === 0 && (
            <p className="p-5 text-sm text-muted-foreground">{t("empty")}</p>
          )}
          {conversations.data?.items.map((item) => (
            <button
              key={item.id}
              aria-pressed={selected === item.id}
              className={`block w-full border-b px-5 py-4 text-start text-sm hover:bg-muted ${selected === item.id ? "bg-accent" : ""}`}
              onClick={() => {
                setSelected(item.id);
                setDetails(null);
              }}
            >
              <span dir="auto" className="block break-words font-medium">
                {item.address}
              </span>
              <span className="mt-1 block text-xs text-muted-foreground">
                {item.channel ?? "whatsapp"}
              </span>
            </button>
          ))}
        </aside>
        <section aria-label={t("thread")} className="min-w-0 p-5 sm:p-7">
          {!selected && <p className="py-12 text-center text-muted-foreground">{t("select")}</p>}
          {selected && messages.isPending && <p role="status">{t("loading")}</p>}
          {messages.error && (
            <div role="alert">
              {t("loadFailed")}
              <Button
                onClick={() => {
                  void messages.refetch();
                }}
              >
                {t("retry")}
              </Button>
            </div>
          )}
          {messages.hasNextPage && (
            <Button
              variant="ghost"
              disabled={messages.isFetchingNextPage}
              onClick={() => {
                void messages.fetchNextPage();
              }}
            >
              {t("older")}
            </Button>
          )}
          {selected && messages.isSuccess && items.length === 0 && (
            <p className="py-12 text-center text-muted-foreground">{t("threadEmpty")}</p>
          )}
          <ol className="space-y-6">
            {[...items].reverse().map((message) =>
              message.kind === "run" ? (
                <li key={message.id}>
                  <RunBlock run={message} />
                </li>
              ) : (
                <li
                  key={message.id}
                  className={`flex flex-col ${message.direction === "outbound" ? "items-end" : "items-start"}`}
                >
                  <p
                    dir="auto"
                    className={`max-w-[90%] whitespace-pre-wrap break-words rounded-xl px-4 py-3 text-sm leading-relaxed ${message.direction === "outbound" ? "bg-primary text-primary-foreground" : "bg-muted"}`}
                  >
                    {message.text}
                  </p>
                  <div className="mt-2 flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
                    <time dateTime={message.createdAt}>
                      {new Date(message.createdAt).toLocaleString(i18n.language)}
                    </time>
                    {message.status && (
                      <span
                        className={`rounded-md border px-2 py-1 ${message.status === "failed" ? "text-destructive" : ""}`}
                      >
                        {t(`statuses.${message.status}`)}
                      </span>
                    )}
                    <button
                      className="rounded px-1 py-1 underline focus-visible:ring-2 focus-visible:ring-ring"
                      onClick={() => {
                        setDetails(message);
                      }}
                    >
                      {t("details")}
                    </button>
                  </div>
                </li>
              ),
            )}
          </ol>
        </section>
      </div>
      <MessageDetailsDrawer
        message={details}
        onClose={() => {
          setDetails(null);
        }}
      />
    </div>
  );
}
