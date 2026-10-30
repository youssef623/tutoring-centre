# Day 77 verification

## Delivered behavior

- Portal GET and POST `/api/portal/chat/messages` use `portal.chat` and require a parent or student actor. Neither accepts a conversation identifier. The actor resolves the conversation and tenant.
- Timeline reads merge bounded inbound and outbound queries, ordered by `(created_at, id)` descending with a continuation cursor. The UI reverses those items for chronological display. Failed outbound replies remain visible.
- New sends return 201; retries return 200 with the original message ID. The internal deduplication key includes the server-resolved conversation ID, so different families can reuse the same client ID independently. This refines Day 76's centre-wide key before exposing the first public chat routes.
- Sends validate nonempty text and a 1000-character limit. A per-user sliding-window policy allows 20 requests per minute. The existing CSRF and session-expiry mechanisms apply.
- Chat keeps drafts and optimistic sends in local component state. Retry reuses the client ID. Polling runs every three seconds while visible and awaiting a reply, and stops after a reply or 60 seconds. Messages render as text.
- Staff can list web and WhatsApp conversations, read both directions, inspect delivery status and failure codes, and open an accessible details drawer. Phone masking remains in the server response. Provider identifiers are excluded.
- Application owns the language-model port and types. Infrastructure translates through IChatClient, maps provider failures to Result errors, enforces deadlines and cancellation, and logs only model name, token counts, duration and outcome.
- Scripted responses come from a JSON file and are refused outside Development and Testing. Explicit live configuration requires a model and API key. ADR 0017 records the provider choice and configuration behavior.

## Repository adaptations

The Month 4 conversation read endpoints and frontend feature were absent in this checkout; only conversation storage and translation placeholders existed. This change supplies the missing staff read surface while preserving the `address` field and established shell, permission and masking conventions. There is no replay control or simulated reply. Day 78 remains responsible for consuming inbound processing intents and generating replies.

No database migration is needed. Assistant execution budgets, history selection and final reply truncation belong to the forthcoming agent loop; Day 77 defines and validates their server options.

## Automated evidence

- Domain regression: 554 passed.
- Application regression: 691 passed.
- Architecture regression: 33 passed, including the vendor dependency prohibition.
- Infrastructure regression: 660 passed, followed by 31 focused checks for the final conversation deduplication and masking changes and 15 final model contract checks.
- Frontend regression: 393 passed across 58 files.
- Full API regression: 942 passed in 28 minutes 35 seconds. The final chat HTTP contracts and endpoint inventory also pass all 12 checks against the latest code.
- Frontend type checking, lint, translation parity and production build pass. Existing Fast Refresh and bundle-size warnings remain.
- OpenAPI and the generated frontend API client are refreshed.

## Security review

The query and endpoint signatures contain no portal conversation ID. Read predicates include the actor's centre and portal user; PostgreSQL tests cover a separate student and another centre. Staff requests are explicitly denied even when a staff role holds `portal.chat`. Staff thread reads retain tenant predicates and return 404 for another centre's conversation. Tests cover server validation, idempotent retries, 429, XSS text rendering, same-timestamp pagination and masked WhatsApp phones beside web participant names. Model adapter logs exclude content, exception objects and keys. Startup validation rejects nonpositive limits, missing live credentials and scripted Production configuration.

## Explicitly deferred by the user

- Live-provider smoke call with a real API key.
- Browser visual QA, desktop/mobile viewport checks, concept comparison and visual fidelity sign-off. The browser host was unavailable; the user chose to leave this pending rather than authorize another browser automation method.

Automated tests do not replace these manual checks. No live model call or screenshot comparison is claimed.
