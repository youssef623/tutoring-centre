import { useTranslation } from "react-i18next";
import { Dialog, DialogContent, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import type { ChatMessage } from "@/features/portal/chat/useChatMessages";

export interface ConversationMessage extends Omit<ChatMessage, "direction" | "status"> {
  direction?: "inbound" | "outbound";
  status?: string | null;
  failureCode?: string | null;
  channel?: string;
  sentAt?: string | null;
  processedAt?: string | null;
  correlationId?: string | null;
}

export function MessageDetailsDrawer({
  message,
  onClose,
}: {
  message: ConversationMessage | null;
  onClose: () => void;
}) {
  const { t } = useTranslation("conversations");
  return (
    <Dialog
      open={message !== null}
      onOpenChange={(open) => {
        if (!open) onClose();
      }}
    >
      <DialogContent className="inset-y-0 start-auto end-0 top-0 h-dvh max-h-dvh w-full max-w-md translate-x-0 translate-y-0 content-start overflow-y-auto rounded-none p-6">
        <DialogTitle>{t("details")}</DialogTitle>
        <DialogDescription>{t("storedDetails")}</DialogDescription>
        {message && (
          <>
            <dl className="space-y-4 text-sm">
              {(
                [
                  ["direction", message.direction ?? "inbound"],
                  ["status", message.status],
                  ["failureCode", message.failureCode],
                  ["createdAt", message.createdAt],
                  ["sentAt", message.sentAt],
                  ["processedAt", message.processedAt],
                  ["channel", message.channel],
                  ["messageId", message.id],
                  ["correlationId", message.correlationId],
                ] as const
              )
                .filter(([, value]) => value != null)
                .map(([key, value]) => (
                  <div key={key}>
                    <dt className="text-muted-foreground">{t(key)}</dt>
                    <dd dir="auto" className="mt-1 break-words">
                      {value}
                    </dd>
                  </div>
                ))}
            </dl>
            <div className="border-t pt-4">
              <h3 className="text-sm text-muted-foreground">{t("text")}</h3>
              <p
                dir="auto"
                className="mt-2 whitespace-pre-wrap break-words text-sm leading-relaxed"
              >
                {message.text}
              </p>
            </div>
          </>
        )}
      </DialogContent>
    </Dialog>
  );
}
