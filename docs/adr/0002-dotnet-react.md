# ADR 0002: .NET backend and React frontend

- **Status:** Accepted
- **Date:** 2026-10-02

## Context

The career target behind this portfolio project is .NET backend roles in Egypt and remote, so the backend technology choice is also a demonstration of that skill set, not just an implementation detail. The domain itself is large and will keep growing over the eight-month build: billing and payments, attendance (manual and QR), scheduling, and an AI assistant that has to call the same authorized use cases as the staff web app. That size and the need for correctness around money and access control benefit from a statically typed language and a mature, battle-tested ORM rather than a dynamically typed stack assembled from smaller libraries. Staff interact with the system through a rich, interactive single-page app (schedules, attendance boards, payment flows), not a handful of server-rendered pages. There is one developer building and maintaining all of this.

## Decision

Backend: ASP.NET Core (.NET 10) with EF Core and PostgreSQL, structured with Clean Architecture (ADR 0001) and a hand-written CQRS pipeline. Frontend: a React + TypeScript single-page app built with Vite, served from the same origin as the API in production (and proxied to it in development — see Day 3), so there is one deployable unit and no cross-origin complexity.

## Alternatives considered

1. **Node.js/TypeScript end to end.** One language across the whole stack lowers context-switching cost for a solo developer, and TypeScript gives static typing on the backend too. Rejected because it doesn't match the career target (.NET backend roles), and the Node.js ecosystem's transactional/ORM tooling (for the kind of billing and payment correctness this domain needs) is less mature than EF Core's.
2. **Angular for the frontend.** Angular's batteries-included structure and strong typing would fit a large, long-lived frontend well. Rejected in favor of React because React's ecosystem for this specific stack — TanStack Router/Query, shadcn/ui — is what was already chosen in Day 3, and React's job-market presence better matches the roles this portfolio targets.

## Consequences

Running two languages means two toolchains, two sets of conventions, and two things that can each independently break — CI (Tasks 4.1–4.2) has to build and test both on every pull request so neither one silently regresses. The backend and frontend are bridged by a generated API client from an OpenAPI spec starting Day 12, rather than hand-written fetch calls and hand-written response types on the frontend; this means the contract between the two languages is generated, not duplicated by hand, so it can't drift silently when the API changes — a changed response shape becomes a type error in the frontend build instead of a runtime bug.
