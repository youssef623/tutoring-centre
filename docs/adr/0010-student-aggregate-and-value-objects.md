# ADR 0010: Student aggregate and value objects

- **Status:** Accepted
- **Date:** 2026-10-12

## Context

Month 3 needs to represent a student, the parents a centre contacts about them, and the link between the
two, before any of it is persisted or exposed (Day 37; storage and endpoints follow Days 38–41). Two separate
design questions came up while building this: how to validate a phone number and a search-friendly name
without scattering the rules that do it, and how to draw the aggregate boundary between a student and the
parents linked to them. See `docs/architecture/academics.md` for the resulting model and the invariant it
protects.

## Alternatives considered

### For the value objects (`PhoneNumber`, `SearchText`)

- **Validating strings in every validator** (each command's FluentValidation rules re-check the phone format
  and each handler re-normalizes a name for search) keeps the Domain free of the concept entirely, but means
  the same regex and the same Arabic letter-folding table exist whichever handler happens to touch a phone or
  a name, with no single place to fix or test them. A rule changed in one validator and missed in another
  would compile and pass unrelated tests, then fail only in production, on exactly the input the missed
  validator should have caught.
- **A database domain type** (a PostgreSQL `DOMAIN` with a `CHECK` constraint for the phone format, a
  generated column for the normalized name) enforces the rule even against a write that bypasses the
  application entirely — the same argument tenant isolation's Layer 3 makes for row-level security. It moves
  business logic (what counts as a valid Egyptian mobile number, how Arabic hamza forms fold together) into
  SQL, where it cannot be unit-tested the way `PhoneNumberTests` and `SearchTextTests` test it here, and ties
  a Month 3 product rule to a specific database engine's extension syntax.
- **A third-party phone-number library** (e.g. a libphonenumber port) gives broader international coverage
  and handles edge cases this hand-written parser does not attempt — extensions, area-code-only dialing
  rules for countries this product does not yet serve. It also pulls in a dependency Month 1's architecture
  rule (`DependencyRuleTests`) would then have to allow into the Domain, for a format this product only needs
  to get right for one country today, with international numbers accepted unchanged rather than deeply
  validated.
- **A value object with a `Create` factory returning a `Result`** (this decision) keeps the rule in exactly
  one place, in the Domain, tested the same way `Centre.Create` and `Subject.Create` already are, with no new
  dependency. It costs an upfront design pass (where do cross-module value objects live, how do they fail) that
  a string-and-validator-per-handler approach defers indefinitely.

### For the aggregate boundary (`Student`, `Parent`, `StudentParentLink`)

- **Three independent tables with the invariant enforced in handlers** (a `students` table, a `parents`
  table, a `student_parent_links` table, with the 1–4-links/one-primary rule checked by the Application
  command that happens to touch it) would work for any single command in isolation, but the invariant would
  then depend on every command that can change a link remembering to re-check it, including ones written
  later by someone who has not read this ADR. A missed check fails silently, the same risk
  `docs/adr/0006-tenant-isolation.md` rejected a purely application-level tenant filter for.
- **One large aggregate containing parents** (`Student` holding a collection of full `Parent` objects, or
  `Parent` holding a collection of `Student`s) would make the invariant trivially easy to enforce in one
  place, but is wrong about what changes together: a parent's phone number changes independently of any one
  student, and the same parent is linked from more than one student (siblings). Nesting one inside the other
  would mean saving a phone-number correction has to load and re-save every student that parent is linked
  to, or saving a student has to carry a parent's full, possibly-stale state along with it.
- **Domain events between separate aggregates** (`Student` raises `ParentLinked`/`ParentUnlinked`; a handler
  elsewhere keeps a denormalized read model in sync) suits a case where the two sides can legitimately
  disagree for a short time, or where more than one consumer needs to react to a link changing. Nothing here
  needs that: the invariant is checked synchronously, inside one `SaveChanges`, the same atomicity argument
  `docs/adr/0008-audit-log-in-transaction.md` makes for the audit log — introducing eventual consistency would
  add a window where the invariant could be observed broken for no benefit this product needs yet.
- **`Student` as the aggregate root over its own `StudentParentLink` children, referencing `Parent` only by
  `ParentId`** (this decision) puts the one invariant that spans multiple rows — link count, no duplicate
  parent, exactly one primary — under the guard of the one type that can see all of it at once, while keeping
  `Parent` free to change on its own schedule, shared across students by identity rather than by reference.

## Decision

`PhoneNumber` and `SearchText` are value objects and a pure function, respectively, in the Domain's shared
`TutoringCentre.Domain.Common` namespace — available to every future module the same way `Entity`, `Result`
and `Error` already are, with no new dependency added. `Student` is the aggregate root for its parent links:
`StudentParentLink` is constructed and mutated only from inside the Domain assembly, and `Student`'s seven
operations are the only way to change the invariant's state. `Parent` is a separate aggregate root, referenced
by `Student` only through a `ParentId`.

## Consequences

The invariant (1–4 links, no duplicate parent, exactly one primary) is provably impossible to break from
outside `Student`, rather than merely untested from outside it — `StudentParentLink`'s `internal` constructor
and mutators mean a violation would be a compile error anywhere outside the Domain assembly, not just a
missing check. The cost is that every parent-link change, including ones Day 40's handlers will add, must go
through `Student`'s own methods; there is no faster path that writes a link row directly. Confirming that a
`ParentId` passed to `Register` or `AddParent` actually belongs to the acting centre is deferred to the
Application layer (Day 39–40), backed by Day 38's composite-key convention for the same reason
`docs/architecture/tenancy.md`'s Layer 4 gives for any cross-aggregate reference: the Domain knows the shape
of a valid link, not whether the other side of it exists.
