# ADR 0019: Agent run inside one transaction

Status: Accepted

## Context

Outbox delivery is at least once. A check followed by an insert races, and a model call can fail or hang. The first conversational assistant needs atomic persistence without a separate leased-run coordinator.

## Decision

RunAgentCommand executes only as a system actor in one centre. The dispatcher opens one transaction. A parameterized INSERT ON CONFLICT DO NOTHING RETURNING claims assistant.agent_runs by (centre_id, inbound_message_id). Concurrent claims wait for the first transaction; after commit, repeat delivery returns AlreadyRan. If the transaction rolls back, a later delivery can claim again.

Context assembly, the bounded AgentLoop, run completion, outbound reply and send intent all belong to that transaction. Only the send command invokes a channel sender. Model failures receive a fixed localized fallback and a failed run record. A run stores counts, timestamps and bounded codes, never message content.

The server's TotalRunTimeout defaults to and cannot exceed 60 seconds. A dispatcher cancellation budget covers transaction begin, handler, save and commit. The handler reserves up to one second, or ten percent for shorter test budgets, for fallback persistence. A bounded for loop enforces MaxSteps, accumulated MaxTotalTokens, CallTimeout and MaxReplyCharacters. Cancellation propagates; non-cooperative model tasks are bounded by WaitAsync. Tool requests stop after one call with assistant.tools_unavailable.

## Consequences

This holds a database connection during a model request, trading connection capacity for straightforward atomic state. No lease column is needed: an uncommitted claim cannot survive a failed transaction. A crash after a billable model response but before commit may repeat the model call; exactly one stored reply is guaranteed, not exactly one external model invocation. Deadline expiry during database work rolls back and lets the relay retry. A model deadline normally leaves time to commit its fallback; database overload may exhaust that reserve too.

In Month 8, revisit three phases: claim and commit, execute outside a transaction, then complete and queue reply. That alternative needs leases, expiry recovery, ownership checks and reconciliation of uncertain model outcomes. It is deliberately deferred until concurrency warrants the complexity.

## Verification

Tests cover simultaneous and sequential claims, real relay redelivery after a lost acknowledgement, one sent reply, fallback, tenant isolation, SQL constraints and privileges. Unit tests cover context determinism, Cairo midnight, token accumulation, step cap, model errors, per-call and total deadlines, cancellation and immediate rejection of tools.
