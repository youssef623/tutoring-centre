import { useInfiniteQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { apiFetch } from "@/api/apiFetch";

export interface ChatMessage {
  id: string;
  direction: "inbound" | "outbound";
  text: string | null;
  status: string | null;
  createdAt: string;
}
interface ChatPageResponse {
  items: ChatMessage[];
  nextCursor: string | null;
}
interface SendResult {
  messageId: string;
  alreadyRecorded: boolean;
}
export interface LocalMessage {
  clientMessageId: string;
  text: string;
  createdAt: string;
  messageId?: string;
}
const queryKey = ["portal-chat"];

export function useChatMessages() {
  const client = useQueryClient();
  const [local, setLocal] = useState<LocalMessage | null>(null);
  const [expected, setExpected] = useState<{ id: string; deadline: number } | null>(null);
  const [visible, setVisible] = useState(document.visibilityState !== "hidden");
  useEffect(() => {
    const change = () => {
      setVisible(document.visibilityState !== "hidden");
    };
    document.addEventListener("visibilitychange", change);
    return () => {
      document.removeEventListener("visibilitychange", change);
    };
  }, []);
  useEffect(() => {
    if (!expected) return;
    const timer = window.setTimeout(
      () => {
        setExpected(null);
      },
      Math.max(0, expected.deadline - Date.now()),
    );
    return () => {
      window.clearTimeout(timer);
    };
  }, [expected]);
  const query = useInfiniteQuery({
    queryKey,
    initialPageParam: null as string | null,
    queryFn: ({ pageParam, signal }) =>
      apiFetch<ChatPageResponse>(
        `/api/portal/chat/messages?pageSize=50${pageParam ? `&cursor=${encodeURIComponent(pageParam)}` : ""}`,
        { signal },
      ),
    getNextPageParam: (page) => page.nextCursor,
    refetchInterval: (state) => {
      if (!visible || !expected || Date.now() >= expected.deadline) return false;
      const items = state.state.data?.pages.flatMap((page) => page.items) ?? [];
      const sentIndex = items.findIndex((item) => item.id === expected.id);
      if (sentIndex >= 0 && items.slice(0, sentIndex).some((item) => item.direction === "outbound"))
        return false;
      return 3000;
    },
    refetchIntervalInBackground: false,
  });
  const send = useMutation({
    mutationFn: (message: LocalMessage) =>
      apiFetch<SendResult>("/api/portal/chat/messages", {
        method: "POST",
        body: JSON.stringify({ clientMessageId: message.clientMessageId, text: message.text }),
      }),
    onSuccess: async (result) => {
      setLocal((current) => (current ? { ...current, messageId: result.messageId } : current));
      setExpected({ id: result.messageId, deadline: Date.now() + 60_000 });
      await client.invalidateQueries({ queryKey });
    },
    retry: false,
  });
  const items = query.data?.pages.flatMap((page) => page.items) ?? [];
  const optimistic = local && !items.some((item) => item.id === local.messageId) ? local : null;
  return {
    ...query,
    items,
    optimistic,
    sendError: send.error,
    sending: send.isPending,
    send: (text: string) => {
      const message = {
        clientMessageId: crypto.randomUUID(),
        text,
        createdAt: new Date().toISOString(),
      };
      setLocal(message);
      send.mutate(message);
    },
    retrySend: () => {
      if (local) send.mutate(local);
    },
  };
}
