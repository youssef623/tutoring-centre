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
