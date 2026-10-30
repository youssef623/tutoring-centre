# Conversational assistant: Week 13

The portal stores inbound messages through SendWebMessageCommand. The relay maps the identifier-only InboundMessageReceived event to ProcessInboundMessageCommand. Identified conversations with a registered channel sender produce RunAgentRequested. WhatsApp messages are marked processed without an agent run.

RunAgentCommand claims one assistant.agent_runs row with an atomic unique insert. The Conversations contract supplies tenant-scoped conversational snapshots and queues the reply intent; Assistant does not depend on Conversations repositories or domain entities. IPortalDirectory supplies the current family's display name. CentreCalendar converts the injected clock to the centre's local date and weekday. ContextBuilder adds the versioned assistant.v1 instructions, bounded oldest-first history and the new inbound message last. It adds no phone numbers or student IDs. Text is limited by Unicode scalars so Arabic and supplementary characters remain intact.

AgentLoop is the only orchestration caller of ILanguageModel. Its bounded for loop enforces step and aggregate token limits. Every request has an empty tool list. Tool requests fail with assistant.tools_unavailable. TotalRunTimeout defaults to 60 seconds and also bounds the dispatcher's transaction; CallTimeout defaults to 20 seconds. The handler reserves a small completion budget. MaxTotalTokens defaults to 6000, MaxSteps to 4, HistoryMessages to 10 and MaxReplyCharacters to 800. Configuration validation rejects invalid limits. Long replies stop at the last sentence boundary below the cap, or the hard Unicode scalar boundary if no sentence ends there.

Success queues one reply to the inbound message. Model error or model deadline queues a fixed English or Arabic fallback and records the failure code. The failed or completed run and reply intent commit together. SendOutboundMessageCommand then uses WebChannelSender to mark the message sent. Portal polling and staff Conversations show both bubbles and statuses. All run reads and writes use forced RLS and the actor's centre. The runtime role has SELECT, INSERT and UPDATE, never DELETE.

Neither prompts nor reply text enter logs, run records or outbox payloads. The model cannot look up attendance, fees or schedules and has no tools. The English instructions require the user's language and style and fewer than three short sentences. Prompt changes require an explicit version and snapshot test update.

## Demo

Use Testing with Assistant__Provider=scripted, Assistant__ScriptPath pointing to frontend/e2e/fixtures/scripted-model.json, and Outbox__RunRelay=true. The fixture contains only synthetic conversational responses. Playwright starts the API in Testing and enables the real relay. Run npx playwright test portal-chat from frontend with the normal seeded database and SEED_PASSWORD configured. It signs in a parent, sends an Arabic greeting, waits for one sent reply, then signs in staff and verifies both bubbles and statuses.

For a local live smoke run, provide Assistant__ApiKey and Assistant__Model through environment variables, set ASSISTANT_E2E_LIVE=1 and run the same journey. No key is needed or allowed in CI. Scripted provider is accepted only in Testing. Keys must never appear in artifacts, logs or committed configuration.

See ADR 0018 for web identity and ADR 0019 for the transaction and future three-phase alternative.
