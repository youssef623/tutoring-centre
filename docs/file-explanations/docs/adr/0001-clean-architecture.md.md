# docs/adr/0001-clean-architecture.md

## Purpose
Architecture Decision Record 0001 (status **Accepted**). It records the decision to split the backend into four Clean Architecture projects with inward-only references and feature-based folders, why, which alternatives were rejected, and the costs accepted.

## Where it fits
Design documentation, linked from `README.md:92`. Its rules are enforced by `tests/TutoringCentre.Architecture.Tests/*` and visible in the four `src/*.csproj` reference lists.

## Walkthrough
- **Context (line 9):**
  - Portfolio project for .NET roles.
  - Three front doors (staff web app, WhatsApp assistant, background jobs) must share rules, e.g. "a payment can't be recorded against a cancelled enrollment".
  - Rules must be testable without a DB.
  - The project spans about eight months.
- **Decision (lines 13–22):**
  - Domain → nothing; Application → Domain; Infrastructure → Application + Domain (Dependency Inversion); Api → Application + Infrastructure, with Infrastructure used **only** in the composition root.
  - Organize by feature (example `Application/Payments/Commands/RecordPayment/`).
  - Boundaries are enforced by architecture tests in CI.
- **Alternatives (lines 26–28):**
  1. A layered monolith without enforcement: rejected because it relies on discipline.
  2. One project per module: rejected as four layers × N modules, too many projects.
  3. Microservices: rejected for distributed-systems cost a single developer doesn't need.
- **Consequences (lines 32–41):**
  - Positive: executable rules, isolated testability, no drift between entry points.
  - Negative: 4–6 files per feature, indirection, mapping boilerplate, an estimated ~15% more backend hours.

## Concepts used
- **Clean Architecture:** concentric layers.
- **Dependency Inversion Principle:** depend on abstractions owned by the inner layer.
- **Vertical/feature slices:** group a use case's files together.
- **Fitness functions.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- References to "Day 2/Day 4" and "Architecture v2 'Complexity these patterns add' analysis" point to an external plan that is not in the repo.
- The feature-folder convention isn't visible yet: the only Application code lives under `Common/`.

## Related files
- [ADR 0002](0002-dotnet-react.md.md)
- [DependencyRuleTests.cs](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
- [ProjectReferenceTests.cs](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
- [Program.cs](../../src/TutoringCentre.Api/Program.cs.md)
