# Later

Ideas that are out of scope for the current slice. Each entry: date, idea, why not now, earliest sensible slice.

| Date | Idea | Why not now | Earliest |
| --- | --- | --- | --- |
| 2026-10-02 | pgAdmin (or another admin tool) in compose.yaml | Not needed to build or test anything yet; Task 2.5 says no extra services | When debugging data by hand becomes frequent |
| 2026-10-02 | Idempotency keys on marked commands | The dispatcher (Day 7) is a single-request, single-transaction pipeline; retried requests aren't a problem yet | Month 6, per `docs/architecture/overview.md`'s dispatcher design section |
| 2026-10-08 | Flaky frontend test: `_authenticated.test.tsx` ("renders the shell with the display name and active centre when a centre is set") failed once in a full-suite clean-clone run (`findByText` timeout) but passed standalone and on an immediate full-suite rerun | Not a Week 1 defect — the file predates this week's work and the failure didn't reproduce; Day 24 is verification-only, so it's recorded rather than chased | Next time it blocks CI, or during general frontend test-suite hardening |
| 2026-10-08 | `npm ci` reports 7 high-severity dependency vulnerabilities (`npm audit`) | Pre-existing, unrelated to Week 1 tenancy work; out of this week's scope | Next dependency-maintenance pass (Dependabot already proposes updates weekly) |
