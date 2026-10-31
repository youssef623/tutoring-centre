# Month 5 checkpoint / Week 15 review

The supplied plan referenced a checkpoint template but did not include one. This record follows the repository's Month 3 local-verification convention: observed evidence and remaining work, without invented roadmap criteria.

## Implemented scope

Parent portal attendance and guarded assistant flows remain the surrounding system. Monthly fees, effective-month schedules, immutable obligations, system jobs/outbox generation, staff keyset reads and permission-gated Billing complete the requested Month 5 billing scope. ADR 0022 records the financial decision. Payments/balances/allocations/proration/adjustment creation/settings, parent finance and assistant finance tools remain out of scope.

## Decisions and security review

- Signed decimal Money preserves future adjustment compatibility; nonnegative fees and positive monthly charges are separate boundaries. Precision/range failures are rejected, never rounded.
- The approved price schedule makes next-month edits truthful, including uncharged current enrollments and historical generation. Charges keep amount/teacher snapshots.
- Monthly uniqueness is partial; composite keys and the enrollment/student triple foreign key prevent tenant and student mismatch. Application grants are insert-only and RLS is forced.
- Generation is system-only; all 27 keys/135 role cells are documented/tested. No finance permission is granted to portal actors or teachers.
- Billing depends on Academics contracts; enrollment only enqueues identifiers, preserving capacity locks. Conflicts are handled on the exact monthly key without swallowing other failures.
- E3/E11 removal is an explicit user scope change: thirteen active cases and no Franco attendance coverage. Remaining gates/floors are retained.
- The existing directory has no inactive-centre model. It currently treats all provisioned centres as active; no unrequested lifecycle UI is added.

## Execution evidence

Release solution build: zero warnings/errors. The consolidated backend run covered Domain 606, Application 795, Infrastructure 714, Api 982, Architecture 45 and Evaluation 85. Grouped failures were fixed and affected checks rerun: Application 343, Infrastructure 57, Architecture 45, Domain 49 and final API 17 passed. The added seed-log regression raises API coverage to 983, for 3,228 unique backend tests covered across the full run and affected reruns. No clean single full-solution green rerun is claimed. Frontend final full run: 417/417 in 63 files, with successful production build, typecheck, lint (existing warnings only) and translation checks.

The verified disposable project was tutoring-d88-checkpoint-20261031, with its own labeled pgdata volume and loopback port 55488; the preexisting developer database was untouched. Empty migrations/seed-twice and a seeded-schema upgrade passed. SQL inspection confirms the composite foreign keys, partial unique index, forced RLS and SELECT/INSERT-only charge grants. Runtime no-tenant reads returned zero across all five requested tables; UPDATE/DELETE were denied for charges and tool_calls.

Tests prove 20 concurrent monthly insertions create one row, batch retry/races, unrelated-failure rollback, transfer/outbox retry behavior, effective fee timing, keyset reads, money shape and role visibility. Controlled removal of the monthly unique index failed the race test; restoring it passed. Replacing the local calendar with UTC failed the Cairo boundary test; restored code passed. Temporary probes were restored byte-for-byte. Additional mutation probes were explicitly skipped by the user.

Real non-browser HTTPS/cookie checks verified 300.00 remains unchanged after a fee edit, invalid fees 400, parent charges 403, foreign charges/timeline 404, refusal business-snapshot equality and parent-session revocation/chat 401 after 52,869 ms at the default cache setting. The scripted messages used here are not live-provider evidence.

The required log scan found seed CLI person-data output (21 matched values in each original seed log). The fix removes display values from 26 diagnostic templates while retaining outcomes/centre slugs/safe codes. A dedicated regression passes; the post-fix scan of five complete captured runtime logs against 89 actual-data/credential patterns found zero matches. Original findings remain private pre-fix artifacts, rather than being represented as clean history.

The 13-case selftest reports categories 4/2/4/3, four parents/six children, unchanged double seeding and safe fresh-container replay. Full retained-cassette replay exits 0: 13/13 quality and both mandatory gates pass; Franco is N/A. This is reduced coverage under the user's instruction, not an improved model-quality claim.

Local evidence: artifacts/day88-build-final.log, day88-regression.log, day88-api-affected.log, day88-infra-affected.log, day88-architecture-final.log, day88-frontend-final.log, day88-selftest.log, day88-replay.log, day88-charge-schema.txt, day88-http-summary.json and day88-log-scan-final.json. Public safe reports are eval-report/report.json and report.md (ignored local artifacts). See docs/runbook.md for all nine scenario statuses.

## Review and merge status

Local code/security review completed; residual risks are recorded in ADR 0022 and later.md. Week 15 checkpoint is implemented with the explicit validation limits below. Remote CI and merge were skipped at user request and are not claimed. Repository CLAUDE.md requires commit/push and leaves PR creation/CI/merge to the user; no PR is silently opened and no merge is implied by this checkpoint.

User steering during validation: **skip live browser journeys**. The authored browser journey is retained, but its execution and manual dashboard/browser observations will remain unverified in this checkpoint. Backend/HTTP/unit/database validation continues; browser outcomes must not be inferred from it.

The user subsequently approved skipping fresh live-model recording / three real-provider messages, manual dashboard triggers, further mutation probes, remote CI/merge verification and live browser journeys. The referenced Month 4 webhook script/route is absent, so that separate manual observation remains unverified. These are explicit limits, not checked-off successes.
