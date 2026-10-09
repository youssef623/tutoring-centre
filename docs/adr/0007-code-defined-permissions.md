# ADR 0007: Code-defined permissions

- **Status:** Draft — the matrix is small and fixed through Month 2 (Days 28, 29, 32); revisit once Day
  29–32's staff, audit and settings permissions are in use and Task 30.10 reviews the accepted disclosures.
- **Date:** 2026-10-09

## Context

Three staff roles (Owner, Secretary, Teacher) and six permissions exist today, and the full matrix between
them is known in advance — it does not vary per centre, per customer or at runtime. Something still has to
answer "does this role hold this permission" on every tenant-scoped request, and that answer has to be
reviewable, testable and impossible to get subtly wrong one row at a time.

## Alternatives considered

- **Database-defined roles and permissions** (tables an admin edits through a UI, each centre able to define
  its own roles or reassign permissions) is the usual shape once customers genuinely need to customize who
  can do what. It costs real weight today: a schema for roles and role-permission assignments, an admin UI
  and its own authorization (who may edit permissions?), migration or seeding for the default matrix, and an
  audit trail for changes to something security-relevant — all for a matrix that, right now, is the same for
  every centre and changes only when a new feature adds a new permission.
- **Code-defined permissions** (this decision) keeps the matrix as a static map, reviewed in a pull request
  like any other code change, covered by an exhaustive table test that states every cell literally rather
  than deriving it from the map under test. Changing what a role may do means a code change, a test change
  and a deploy — slower than an admin UI, but every change is attributable to a commit and a review, and
  there is no runtime path for a bug or a mistaken click to silently grant a permission.

## Decision

The permission catalogue (`Permissions`) and the role-permission map (`RolePermissions`) are fixed in code,
in the Domain layer, with no role or permission tables. `RolePermissionsTests` states all 18 role x
permission cells literally — expected values are never computed from the map it tests — so the exhaustive
table doubles as the audit: every cell is a decision someone can see in a diff. See
`docs/architecture/authorization.md` for the matrix and how it is enforced.

## Consequences

Adding a permission or changing what a role holds requires a code change, not an admin action — acceptable
while the matrix is small and shared across every centre, and arguably safer than making it editable before
there is a real need to. No centre can define its own roles or reassign permissions; if that becomes a real
requirement, migrating to database-defined roles later remains possible; it does not have to be decided now.
This ADR stays in draft because the matrix is still growing (Days 29 and 32 add the staff, audit and settings
permissions this document already lists but leaves unused) and because Task 30.10 is where the project
revisits accepted disclosures about the authorization model — this decision should be reviewed once that
later picture is complete, not accepted prematurely on three days of evidence.
