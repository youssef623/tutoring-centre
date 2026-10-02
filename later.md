# Later

Ideas that are out of scope for the current slice. Each entry: date, idea, why not now, earliest sensible slice.

| Date | Idea | Why not now | Earliest |
| --- | --- | --- | --- |
| 2026-10-02 | pgAdmin (or another admin tool) in compose.yaml | Not needed to build or test anything yet; Task 2.5 says no extra services | When debugging data by hand becomes frequent |
| 2026-10-02 | Idempotency keys on marked commands | The dispatcher (Day 7) is a single-request, single-transaction pipeline; retried requests aren't a problem yet | Month 6, per `docs/architecture/overview.md`'s dispatcher design section |
