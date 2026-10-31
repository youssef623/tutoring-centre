# Day 84 verification

The expected decisions and codes were reviewed against the supplied A1–A10 table. A4 attempts both unregistered tools. Guard-only A7/A8 have no tool evidence rows; agent_runs.guard_reason is their evidence. A6 uses the production defaults and offers 100 calls at once, continuing indefinitely if asked again. The run ends after one model step, six bounded attendance reads and one capped attempt.

Automated checks performed:

- Application: 792 tests pass, including ten gateway attacks and twelve hostile-model contract checks.
- API: the full regression passes all 968 tests. The final ten HTTP/outbox/PostgreSQL attacks, same-count snapshot self-test and extended same-parent walkthrough also pass in focused reruns after the cap fix. Replies, POST/poll response bodies and captured API/relay logs contain no other student's full name or GUID. Capture begins after fixture setup and sign-in, excluding seed logs.
- Domain: 557 tests pass. Architecture: 40 tests pass.
- Infrastructure: all 683 tests pass on the final implementation. The first run exposed two old-code expectations; those literals were aligned with the supplied Day 84 specification before the full rerun.
- Frontend: 402 tests pass across 60 files with `npm run test:ci -- --reporter=dot --maxWorkers=2`. The unrestricted-worker run failed, including timeouts, under concurrent regression load; the two-worker retry passed.

Local mutation checks were run separately and each edit was restored:

| Disabled control | Observed failing case |
| --- | --- |
| Handle resolution | A1 |
| Closed schema | A2, A3, A9 |
| Registry allow-list | A4 |
| Tool call cap | A6 |
| Stop inside a tool batch | A6 |
| Foreign-name guard | A7 |
| Identifier guard | A8 |
| Temporary test attendance tool writes student names | A6 snapshot equality fails: same 18 rows, different SHA-256 content hash |

No mutation source or test-only writing tool is committed. DatabaseSnapshot discovers every user table, includes audit and bypasses RLS only for test assertions. It excludes exactly the six permitted bookkeeping tables. Both row counts and content hashes are compared, and new/dropped tables change the comparison too.

The same-parent HTTP walkthrough covers an authorized answer from marked attendance, followed by an Omar refusal, unchanged business data and a parent `/api/students` 403. A browser-based manual walkthrough, including Maadi-to-Nile and teacher-before/after scenarios, has not been performed. It remains an explicit manual completion item; HTTP tests are not reported as manual UI verification.
