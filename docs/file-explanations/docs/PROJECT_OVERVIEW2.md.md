# docs/PROJECT_OVERVIEW2.md

## Purpose

The current teaching overview of the repository: the original 15 sections and appendices, corrected in place and extended for the merged authentication, session, CSRF, rate-limiting, membership, OpenAPI-client, internationalisation and end-to-end-testing work.

## Where It Fits

Documentation (docs). Starts as an exact copy of `docs/PROJECT_OVERVIEW.md` and keeps all of its content; wrong statements are corrected where they stood and new material is added at the end of the relevant sections. Linked from `docs/file-explanations/INDEX.md` and the area guides, and referenced by the concept links in every per-file explanation.

## Walkthrough

Structure (4018 lines): the same 15 sections and two appendices as the original, in the same order. Added or extended: **1.4** (a current-state table classifying every sign-in, session, tenant and frontend item as complete, partial, stub, unused, TODO, broken or unverified, plus the login, centre-selection and app-shell flows); **2** (new NuGet and npm packages, Playwright, the `e2e` CI job); **3** (the three Mermaid diagrams extended with cookie authentication, antiforgery, rate limiting and the session revalidation callback); **4** and **5** (file map and startup steps); **6.24-6.35** (new concept sections: cookie authentication and server-side sessions, CSRF double-submit, rate limiting and lockout, session revalidation, identity and membership modelling, authorization by default and the tenant gate, architecture rules beyond dependencies, contract-first client generation, frontend sessions and route guards, React Hook Form and Zod, internationalisation and RTL, Playwright), each in the full format; **7.4-7.14** (the authentication system in detail, including 'what does not exist'); **8** (the real middleware order, a 'what breaks if the order changes' table and traces 8.5-8.6); **9.6** (the `identity` schema and memberships); **10.6-10.10** (sign-in walkthrough and related traces); **11-15** and the appendices (CI jobs, environment variables, test counts, quality findings 1-32, glossary, learning path). Statements that the merge made false were corrected where they stood; nothing from the copy was removed.

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
