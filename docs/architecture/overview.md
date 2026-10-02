# Architecture overview

## Failures: results vs exceptions

**Rule.** Expected business outcomes are `Result` failures returned as values. Bugs and infrastructure faults are exceptions, handled once by a global handler.

| Situation | Mechanism | Example |
| --- | --- | --- |
| Invalid input | `Result` failure, kind `Validation` | slug `"Bad Slug"` → `centre.slug_invalid` |
| Something requested doesn't exist | `NotFound` | unknown centre (from Week 2) |
| Conflicts with existing data | `Conflict` | duplicate slug (Day 9) |
| A business rule says no | `Rule` | enrolling into a full group (Month 3) |
| The caller isn't allowed | `Forbidden` | non-system actor creating a centre (Week 2) |
| A bug | exception | reading `Value` of a failed `Result` |
| An infrastructure fault | exception | database unreachable, timeout |

**Kinds.** `Validation`, `NotFound`, `Conflict`, `Rule`, `Forbidden` — a closed set. The Api maps each kind to exactly one HTTP status (Day 11); adding a kind is an API-contract change.

**Codes.** `<feature>.<reason>`: lowercase feature, snake_case reason — `centre.slug_invalid`, `centre.name_too_long`. Codes are a public contract that the frontend translates; renaming one is a breaking change.

**Messages** are for developers and logs. Users see translated text chosen by the code.

**Safety.** Codes and messages never contain internals: no stack traces, SQL, file paths or secrets.

## Dispatcher design (implemented on Day 7)

Two pipelines, one per CQRS interface. Tenant and permission checks (Month 2) plug in **after validation, before BEGIN**, in both pipelines.

```
Send(command):  validate ─fail→ return failure (no transaction)
                └ok→ BEGIN (read-write) → handler.HandleAsync
                     ├ failure result → ROLLBACK → return failure
                     ├ exception      → ROLLBACK → rethrow (global handler → 500)
                     └ success        → SaveChanges → COMMIT → return success

Query(query):   validate ─fail→ return failure
                └ok→ BEGIN READ ONLY → handler.HandleAsync → COMMIT → return result (never SaveChanges)
```

**Why validation runs before BEGIN.** Invalid input needs no database work, so no transaction (and no connection) is opened for it.

**Why the transaction starts before the handler, not just around SaveChanges.** Repositories will take row locks (`FOR UPDATE`) while the handler reads, and Month 2 sets the PostgreSQL row-level-security tenant inside the transaction; both must cover the whole handler, not just the save.

**What happens when a query handler throws.** End the read-only transaction (roll back) and rethrow; the global handler returns 500.

**Who calls SaveChanges.** Only the command pipeline, exactly once, after a successful handler; queries never call it.

**Where tenant and permission checks go.** After validation, before BEGIN, in both pipelines (Month 2). Idempotency for marked commands comes later (Month 6).
