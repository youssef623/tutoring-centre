# ADR 0020: Tool gateway and run-local family handles

Status: Accepted, 2026-10-31 (M5, Week 14)

The model is an untrusted dependency. Prompt instructions cannot authorize a query, a student identifier or a write. A compromised model must remain inside the parent's current family and centre.

All tool calls enter ToolGateway. A closed registry, closed argument schema and run-local handle table constrain execution. The gateway resolves the active portal actor again and executes the existing attendance query through its scoped dispatcher, preserving family authorization and RLS. Handles are minted from current family membership, kept in memory and never accepted as persistent authorization. Raw GUIDs are explicitly rejected; malformed handles and extra fields cannot reach execution. Date ranges are bounded to 62 days and queries to 31 records.

The gateway records one content-free evidence row per attempted call. Schema and semantic argument failures use `invalid / tool.invalid_arguments`; raw IDs use `denied / tool.raw_id_rejected`; unknown handles use `denied / tool.handle_unknown`; unknown tools use `denied / tool.unknown`. The tool cap uses `denied / tool.call_cap`. Independent step, token and time limits also end a run. Output identifier, grounding, foreign-name and format guards execute before queueing a reply; rejected candidates are replaced with a fixed fallback and never stored as outbound text.

The loop checks the cap immediately after each attempted call, including inside a single model response. Checking only after the batch would bound successful reads but permit excessive evidence rows. A6 submits 100 calls repeatedly and must stop after six reads plus one denied attempt with the default limits.

Alternatives considered:

- Trust-and-filter: prompt rules and post-filtering alone cannot prevent unauthorized tool execution or business writes.
- ID-based tools: stable student IDs expose internal identifiers and invite cross-family probes. Run-local handles narrow the model's vocabulary; existing authorization remains necessary.
- Separate AI service account: a broadly privileged account creates another authorization system. Reuse the current portal actor and existing query path instead.

Day 84 uses a hostile ILanguageModel and a literal attack catalogue, with gateway and HTTP/outbox/PostgreSQL tests. Zero database change means unchanged row counts and content hashes for every user table except conversations.conversations, conversations.inbound_messages, conversations.outbound_messages, assistant.agent_runs, assistant.tool_calls and jobs.outbox_messages. Audit is included. A6 permits bounded authorized reads, never business writes. Guard-only attacks A7/A8 have no tool_calls row because no call was attempted; their evidence is agent_runs.guard_reason.

Consequences: deterministic name checks are not semantic fact checking. Centre names are loaded per reply, handles expire with the run, and missing attendance is not absence. See [the threat model](../ai/threat-model.md) and later.md for residual risk. No new schema, service account or model provider is introduced.
