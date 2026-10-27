# `tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): A brand-new environment (CI, production) must be buildable from the migrations alone.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 42 lines and 1,662 bytes. Source hash: `bde001def27938f642c59f71e346b0b390ab0c7e7dc839a44da79f665dda1ea8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs#L6): `public sealed class MigrationTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs#L9): `public async Task Migrations_AppliedToEmptyDatabase_CreateCentresTable()`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs#L17): `public async Task Migrations_AppliedToEmptyDatabase_CreateUniqueSlugIndex()`
- [Line 27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs#L27): `public async Task Migrations_AppliedToEmptyDatabase_CreateDefaultLocaleCheckConstraint()`
- [Line 36](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs#L36): `public async Task Migrations_AppliedToEmptyDatabase_KeepHistoryTableInPlatformSchema()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>A brand-new environment (CI, production) must be buildable from the migrations alone.</summary>
public sealed class MigrationTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_CreateCentresTable()
    {
        var exists = await Fixture.ScalarAsync<bool>("select to_regclass('platform.centres') is not null");

        Assert.True(exists);
    }

    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_CreateUniqueSlugIndex()
    {
        var indexDefinition = await Fixture.ScalarAsync<string>(
            "select indexdef from pg_indexes where schemaname = 'platform' and tablename = 'centres' and indexname = 'ux_centres_slug'");

        Assert.StartsWith("CREATE UNIQUE INDEX", indexDefinition, StringComparison.Ordinal);
        Assert.Contains("slug", indexDefinition, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_CreateDefaultLocaleCheckConstraint()
    {
        var count = await Fixture.ScalarAsync<long>(
            "select count(*) from information_schema.check_constraints where constraint_schema = 'platform' and constraint_name = 'ck_centres_default_locale'");

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_KeepHistoryTableInPlatformSchema()
    {
        var exists = await Fixture.ScalarAsync<bool>("select to_regclass('platform.__ef_migrations_history') is not null");

        Assert.True(exists);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
