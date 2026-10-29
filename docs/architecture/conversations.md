# Conversations

Day 70 adds channel registration and transactional inbound storage. HTTP webhooks,
sender identity, processing mappings and the conversation list belong to later slices.

## Storage and tenancy

`channel_accounts` is tenant-owned in EF, including its query filter and write guard.
It deliberately has no RLS: the edge must resolve the receiving provider account before
an actor exists. `IInboundChannelResolver` is the only filter bypass for this table;
it selects active accounts by provider and external ID and returns IDs only.
Registration is system-only, tenant-scoped and audited. Display phones are masked.

The other two tables have enabled, forced RLS, composite tenant foreign keys and no
runtime DELETE privilege. `conversations.ingest` is held by no staff role.

## Insert derivation and comparison

The contract SQL was present in the original task prompt, so this is not a blind
exercise. Before implementing the store, the table keys imply these statements:

```sql
INSERT INTO conversations.conversations
  (id, centre_id, channel_account_id, counterpart_address, counterpart_phone,
   identity_status, last_message_at, created_at)
VALUES (@newId, @centre, @account, @address, @phone, 'unresolved', @at, @now)
ON CONFLICT (centre_id, channel_account_id, counterpart_address)
DO UPDATE SET last_message_at = GREATEST(conversations.conversations.last_message_at, EXCLUDED.last_message_at)
RETURNING id;

INSERT INTO conversations.inbound_messages
  (id, centre_id, conversation_id, provider, provider_message_id, message_type,
   text_body, provider_timestamp, received_at, status, created_at)
VALUES (@newId, @centre, @conversationId, @provider, @providerMessageId, @type,
        @text, @providerTs, @now, 'received', @now)
ON CONFLICT (centre_id, provider, provider_message_id) DO NOTHING
RETURNING id;
```

Comparison with Task 70.5: columns, conflict targets, `GREATEST`, `DO NOTHING` and
`RETURNING id` match. Both statements must use parameters, the actor's centre and
the dispatcher's current transaction. No check-then-insert or unique-error catch
belongs in this path: a unique violation aborts a PostgreSQL transaction.

Only an inserted message enqueues `conversations.inbound_message_received` with
`MessageId`. The EF outbox save and raw inserts share one transaction, so a failed
save rolls everything back. No processing mapping is registered on Day 70.
