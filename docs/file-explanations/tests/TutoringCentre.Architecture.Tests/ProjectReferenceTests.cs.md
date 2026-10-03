# tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs

## Purpose
The **declared-reference** rule. It fails when a `.csproj` *declares* a `ProjectReference` outside the allowed set, even if no code uses it (lines 3–4). It sees what `DependencyRuleTests` cannot.

## Where it fits
Architecture.Tests. It uses `ProjectFiles.ReadProjectReferences`. The allowed sets match the README and ADR diagram ("diagram B", line 7).

## Walkthrough
- **Lines 8–11, allowed sets (ordinal-sorted):**
  - Domain `[]`;
  - Application `["TutoringCentre.Domain"]`;
  - Infrastructure `["TutoringCentre.Application","TutoringCentre.Domain"]`;
  - Api `["TutoringCentre.Application","TutoringCentre.Infrastructure"]`.
- **Lines 13–27:** four `[Fact]` tests, each `Assert.Equal(expected, ReadProjectReferences(name))`. It's an exact match: both extra and missing references fail.

## Concepts used
- **Fitness functions on build metadata.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Keep the expected arrays ordinally sorted.** Otherwise the test fails even when the references are correct (old `tests/.semantic.md:97`).
- **Only `ProjectReference` is checked.** `PackageReference` and `FrameworkReference` are not checked here. The IL test covers their usage.

## Related files
- [ProjectFiles.cs](ProjectFiles.cs.md)
- [DependencyRuleTests.cs](DependencyRuleTests.cs.md)
