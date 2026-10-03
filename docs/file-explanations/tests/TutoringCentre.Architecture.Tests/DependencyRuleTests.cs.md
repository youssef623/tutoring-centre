# tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs

## Purpose
**Type-level** architecture rules. They fail when compiled code in a layer *uses* a forbidden namespace (line 6). NetArchTest reads each assembly's IL.

## Where it fits
Architecture.Tests. It loads the assemblies through each project's `AssemblyMarker` (allowed by `InternalsVisibleTo`). It complements `ProjectReferenceTests`, which checks declared references.

## Walkthrough
- **Lines 9–11:** assemblies via `typeof(TutoringCentre.X.AssemblyMarker).Assembly`.
- **Lines 13–21:** `ForbiddenForDomain`: the other three layers, `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore`, `Npgsql`.
- **Lines 23–30:** `ForbiddenForApplication`: Infrastructure, Api, EF Core, ASP.NET Core, Npgsql.
- **Lines 32–35:** `ForbiddenForInfrastructure`: Api.
- **Lines 37–47:** three `[Fact]` tests, one per array.
- **Lines 49–61, `AssertNoDependencies`:**
  1. `Types.InAssembly(assembly)`.
  2. `Assert.NotEmpty(types.GetTypes())`, which guards against scanning an empty or wrong assembly and passing vacuously.
  3. `ShouldNot().HaveDependencyOnAny(forbidden).GetResult()`.
  4. Assert success, listing `FailingTypeNames` in the message.

## Concepts used
- **Fitness functions.**
- **IL inspection:** NetArchTest uses Mono.Cecil, so it sees field types, method calls and base types in compiled code.
- **Namespace-prefix matching:** `"Microsoft.AspNetCore"` covers all sub-namespaces.

## Data and control flow
Assembly → type list → dependency scan → pass/fail with the offending type names.

## Configuration and environment
None.

## Gotchas and issues
- **Unused references are invisible.** An unused `ProjectReference` is dropped from IL, so this test can't see it. That's why `ProjectReferenceTests` exists.
- **No rule for the Api project.** Nothing stops endpoint code from using Infrastructure types directly.
- **Application may use `Microsoft.Extensions.*`** (DI, Logging, FluentValidation). That is deliberate, but there is no allow-list documenting it.

## Related files
- [ProjectReferenceTests.cs](ProjectReferenceTests.cs.md)
- [ProjectFiles.cs](ProjectFiles.cs.md)
- [ADR 0001](../../docs/adr/0001-clean-architecture.md.md)
