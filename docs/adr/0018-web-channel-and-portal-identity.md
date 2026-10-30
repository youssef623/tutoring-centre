# ADR 0018: Web channel and portal identity

Status: Accepted

## Context

Portal messages must belong to the signed-in family's conversation. Phone-based identity resolution cannot identify a browser session safely. A browser also needs a durable timeline and retry behavior rather than a synchronous model call.

## Decision

The authenticated portal actor determines the centre, user and parent or student subject. One web conversation exists per centre, channel account and portal user. Its identified subject is fixed; a request cannot nominate another family. Web conversations contain no phone counterpart. Portal directory contracts supply only the current subject's active display name and role to context assembly.

Sending stores the inbound text and an identifier-only inbound event atomically. Repeated client IDs return the original message. The relay processes the inbound event, schedules an agent run only for an identified conversation with a registered sender, and delivers the resulting outbound intent through WebChannelSender. Portal polling and staff Conversations read the same stored timeline and delivery states. Staff cannot post through the portal chat route.

## Consequences

The assistant never receives student IDs, phone metadata, another family's records or executable tools. User-authored text can itself contain personal information; context assembly does not extract or add identity identifiers. Only the family's own display name is added. Prompts and replies are not logged or copied to outbox payloads. Unresolved or WhatsApp conversations do not schedule a run because no WhatsApp sender is registered.

Polling adds relay latency, but committed messages and replies survive browser refreshes. A unique outbound reply key provides a second duplicate-reply backstop.
