# docs/PROJECT_OVERVIEW2.md

## Purpose

The current teaching overview of the repository: the original 15 sections and appendices, corrected in place and extended for the merged authentication, session, CSRF, rate-limiting, membership, OpenAPI-client, internationalisation and end-to-end-testing work.

## Where It Fits

Documentation (docs). Starts as an exact copy of `docs/PROJECT_OVERVIEW.md` and keeps all of its content; wrong statements are corrected where they stood and new material is added at the end of the relevant sections. Linked from `docs/file-explanations/INDEX.md` and the area guides, and referenced by the concept links in every per-file explanation.

## Walkthrough

See the section map in the first lines of the document and in `docs/file-explanations/INDEX.md`; this explanation is refreshed when the overview changes.

## Concepts Used

### Architecture Decision Records and documentation as code

#### What it means

An ADR records a decision, its context, alternatives and consequences, so the *why* survives the people who made it. Docs kept in the repo are versioned with the code.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

A long-form teaching document.

#### How it works here

Whole file.

#### Why it matters here

Complements the per-file explanations.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`docs/PROJECT_OVERVIEW.md`](PROJECT_OVERVIEW.md.md)
- [`README.md`](../README.md.md)
- [`docs/architecture/authentication.md`](architecture/authentication.md.md)
