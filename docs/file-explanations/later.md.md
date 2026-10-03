# later.md

## Purpose

Parking lot for ideas deliberately postponed, with the reason and the earliest sensible time.

## Where It Fits

Root documentation note; referenced by no code. Entries refer to `compose.yaml` and the dispatcher design in `docs/architecture/overview.md`.

## Walkthrough

A table with columns Date / Idea / Why not now / Earliest. Row 1 (2026-10-02): pgAdmin in `compose.yaml` - not needed yet, 'Task 2.5 says no extra services'. Row 2 (2026-10-02): idempotency keys on marked commands - the dispatcher is a single-request, single-transaction pipeline so retries are not a problem yet; earliest 'Month 6'. Shows the author's habit of recording deferrals instead of building speculatively.

## Concepts Used

This file introduces no concept that needs a tutorial beyond what its walkthrough already explains.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

References a plan ('Task 2.5', 'Month 6') that is not in the repository.

## Related Files

- [`compose.yaml`](compose.yaml.md)
- [`docs/architecture/overview.md`](docs/architecture/overview.md.md)
