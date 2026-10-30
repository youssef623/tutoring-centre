# Day 78 and Week 13 review

## Delivered behavior

A portal send stores an inbound message and identifier-only intent. The relay processes that message, schedules a system-only RunAgentCommand for an identified conversation with a sender, claims one run, builds deterministic context and calls the bounded model loop. The run, one outbound reply and its send intent commit together. WebChannelSender records delivery and the portal and staff Conversations show the same bubbles and statuses. WhatsApp processing does not schedule an agent run.

The checkout had a deferred inbound event but no Month 4 processing handler. Day 78 installs that consumer without changing its existing event type. Assistant accesses Conversations through a published snapshot and reply-intent contract so the module boundaries remain enforced.

## Evidence

- Domain: 556 passing tests, including forward-only run transitions and invalid counts, codes and timestamps.
- Application: 708 passing tests, including context order and truncation, Arabic preservation, Cairo midnight, prompt-version snapshot, loop limits, cancellation, command idempotency and fallback.
- Infrastructure: 671 passing tests, including simultaneous and sequential claims, real relay redelivery after a lost acknowledgement, one sent reply, Arabic failure fallback, tenant isolation, constraints and runtime grants.
- Architecture: 33 passing tests, including module boundaries, raw SQL allow-list, outbox payloads and vendor-free application code.
- Frontend: 394 passing tests across 58 files with two workers. Type checking and lint pass; the existing 50 Fast Refresh warnings remain.
- Scripted browser demo: the parent signs in, sends an Arabic greeting, sees exactly one sent reply, and staff sees both bubbles with processed/sent statuses.
- Browser regression: all 24 Month 1-4 and Week 13 journeys pass. The final two-sentence scripted reply also passes the focused chat journey.
- Full API regression: stopped at the user's request before completion. The focused HTTP relay test passes; the full 943-test suite is not claimed green.

The claim mutation check temporarily removes ON CONFLICT. The parallel-claim test then fails with PostgreSQL 23505 on ux_agent_runs_inbound_message. The production SQL is restored automatically; the complete infrastructure suite passes with it restored. Additional application tests prove the server budget cancels a stalled database save or commit, rolls back with an uncancelled token, and preserves usage from earlier model steps when the total deadline expires. The step-cap theory exercises MaxSteps values of 1 and 3 and observes exactly that number of calls before assistant.step_limit.

The real browser exposed a pre-existing polling gap: the portal stopped polling when a queued reply appeared, before the separate delivery command marked it sent. Polling now continues until that reply leaves queued status or the existing visibility/deadline bounds stop it. A focused regression proves queued becomes sent without a refresh.

## Security and manual code review

The loop is a bounded for. Limits come from validated server configuration. The dispatcher bounds transaction begin, execution, save and commit by TotalRunTimeout, at most 60 seconds. Context and model work leave a small completion reserve. Each call also has its own timeout, and WaitAsync bounds a non-cooperative model task. Caller cancellation propagates. Cumulative usage and reply length are bounded; tool requests stop after one call with assistant.tools_unavailable.

The handler uses the injected clock and CentreCalendar, never DateTime.UtcNow. Prompts contain the current family's display name and role, centre-local date and bounded conversational history. No identity phone metadata or student IDs are added. User-authored text may itself contain personal information; it remains within that family's context. Run rows contain only identifiers, counts, timestamps and bounded codes. Prompts, replies, provider exception objects and keys are excluded from model logs and outbox payloads.

Only a system actor in a centre can run the assistant. Tenant predicates, composite foreign keys and forced RLS protect run and conversation access. Runtime grants omit DELETE. The outbound unique reply key supplements the run claim. The reply destination is always reloaded from the originating inbound message. The model receives no tools and cannot look up attendance, fees or schedules or change records.

The happy path is exercised through the real browser and relay. HTTP tests deny staff chat posts; tenant tests prevent cross-centre runs and reads. Loop tests prove the reply cap and both deadlines. Scripted model failures prove the fixed localized fallback and assistant.model_error without using a bad real key.

## Decisions and follow-up

ADR 0018 records web identity and durable delivery. ADR 0019 records the connection held during the model call, bounded transaction and Month 8 claim/run/complete alternative. A rollback after a model response can repeat a billable call; the database guarantees one stored reply, not exactly one external invocation. This trade-off is recorded in later.md.

The live-provider demo is explicitly skipped at the user's request. No real model call or real-key verification is claimed. The browser and CI use the scripted provider in Testing; the fixture contains synthetic responses only.
