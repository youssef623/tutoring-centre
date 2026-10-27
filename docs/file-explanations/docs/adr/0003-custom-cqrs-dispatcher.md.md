# `docs/adr/0003-custom-cqrs-dispatcher.md`

## Purpose

Documentation covering ADR 0003: Hand-written CQRS dispatcher; Context; Decision; Alternatives considered; Consequences. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../adr/0003-custom-cqrs-dispatcher.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 30 lines and 2,529 bytes. Source hash: `29c292a3ff8af56ea2a9b9005aba92cc0234a22071495b07608fc541a3c7d1e1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: ADR 0003: Hand-written CQRS dispatcher
- Line 6: Context
- Line 10: Decision
- Line 19: Alternatives considered
- Line 25: Consequences

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# ADR 0003: Hand-written CQRS dispatcher

- **Status:** Accepted
- **Date:** 2026-10-04

## Context

Every use case needs the same cross-cutting steps in a fixed order: validate the request, open the right kind of transaction, run the handler, commit or roll back, and log the outcome once. From Month 2 two more steps join them — tenant resolution and permission checks — and they must be impossible to forget on any use case. Handlers are invoked from several clients (HTTP, the seed CLI, later background jobs and an AI assistant), so the steps cannot live in HTTP middleware or endpoints.

## Decision

A small hand-written `Dispatcher` in the Application layer with two explicit pipelines:

- Commands: validate → begin read-write transaction → handler → on failure result roll back, otherwise save once and commit; an exception rolls back and is rethrown to the global exception handler.
- Queries: validate → begin read-only transaction → handler → commit; never saves.

Handlers are resolved by closed generic type (`ICommandHandler<TCommand, TResponse>` / `IQueryHandler<TQuery, TResponse>`); validators are discovered by assembly scanning; handlers return `Result<T>`. The tenant and permission steps will be added between validation and `BeginAsync`.

## Alternatives considered

1. **MediatR with pipeline behaviours.** Mature, but its behaviours are implicit and its licensing changed; the pipeline is the core of this project and worth owning and learning.
2. **Decorators around each handler.** Explicit, but each decorator must be registered per handler and ordering becomes configuration instead of code.
3. **Calling services directly from endpoints.** No shared pipeline; every endpoint would repeat transactions, validation and logging.

```

The excerpt stops at line 24; use the original file for the remaining 6 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
