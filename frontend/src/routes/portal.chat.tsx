import { createFileRoute } from "@tanstack/react-router";
import { ChatPage } from "@/features/portal/chat/ChatPage";

export const Route = createFileRoute("/portal/chat")({ component: ChatPage });
