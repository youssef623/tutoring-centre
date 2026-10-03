# later.md

## Purpose
A parking lot for ideas deliberately postponed. Each row records the date, the idea, why not now, and the earliest sensible slice (line 3). It keeps scope creep out of the current day's work while still capturing the thought.

## Where it fits
Project-management documentation. It refers to `docs/architecture/overview.md` (the dispatcher design section) and to plan items "Task 2.5" and "Day 7".

## Walkthrough
| Line | Idea | Why not now | Earliest |
| --- | --- | --- | --- |
| 7 | pgAdmin (or another admin tool) in `compose.yaml` | not needed yet; "Task 2.5 says no extra services" | when debugging data by hand becomes frequent |
| 8 | Idempotency keys on marked commands | the dispatcher is single-request, single-transaction; retries aren't a problem yet | Month 6 |

## Concepts used
- **Idempotency keys:** a client-supplied key that lets the server recognise a retried command and not apply it twice.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
"Task 2.5" refers to an external plan document that is not in the repo.

## Related files
- [docs/architecture/overview.md](docs/architecture/overview.md.md)
- [compose.yaml](compose.yaml.md)
