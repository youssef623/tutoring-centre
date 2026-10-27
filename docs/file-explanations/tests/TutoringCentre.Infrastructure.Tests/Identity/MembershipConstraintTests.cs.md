# `tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Proves the database itself — not just application code — prevents duplicate, orphaned or cascaded identity data.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 109 lines and 4,741 bytes. Source hash: `b5b4a8fcf19fdd237b9841b8ed70ee3141fac0b520739372bc6e033484f35390`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Npgsql;`
- Line 4: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 5: `using TutoringCentre.Application.Common.Security;`
- Line 6: `using TutoringCentre.Domain.Centres;`
- Line 7: `using TutoringCentre.Domain.Identity;`
- Line 8: `using TutoringCentre.Infrastructure.Identity;`
- Line 9: `using TutoringCentre.Infrastructure.Persistence;`
- Line 10: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L15): `public sealed class MembershipConstraintTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L18): `public async Task Migrations_AppliedToEmptyDatabase_CreateExactlyTheFiveIdentityTablesAndNoRoleTables()`
- [Line 37](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L37): `public async Task SaveChanges_SecondMembershipForSameCentreAndUser_ViolatesUniqueIndex()`
- [Line 55](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L55): `public async Task SaveChanges_MembershipForNonExistentCentre_ViolatesForeignKey()`
- [Line 69](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L69): `public async Task Delete_CentreWithAMembership_IsRejected()`
- [Line 87](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L87): `private async Task<Guid> CreateCentreAsync()`
- [Line 94](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs#L94): `private static async Task<Guid> CreateUserAsync(AppDbContext context)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Proves the database itself — not just application code — prevents duplicate, orphaned or cascaded identity data.</summary>
public sealed class MembershipConstraintTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_CreateExactlyTheFiveIdentityTablesAndNoRoleTables()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "select table_name from information_schema.tables where table_schema = 'identity' order by table_name",
            connection);
        await using var reader = await command.ExecuteReaderAsync();

        var tables = new List<string>();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }

        Assert.Equal(["memberships", "user_claims", "user_logins", "user_tokens", "users"], tables);
    }

    [Fact]
    public async Task SaveChanges_SecondMembershipForSameCentreAndUser_ViolatesUniqueIndex()
    {
        using var scope = Fixture.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var centreId = await CreateCentreAsync();
        var userId = await CreateUserAsync(context);

        context.Add(Membership.Create(userId, centreId, StaffRole.Teacher).Value);
        await context.SaveChangesAsync();

        context.Add(Membership.Create(userId, centreId, StaffRole.Secretary).Value);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        Assert.IsType<PostgresException>(exception.InnerException);
        Assert.Equal("23505", ((PostgresException)exception.InnerException).SqlState);
    }

    [Fact]
    public async Task SaveChanges_MembershipForNonExistentCentre_ViolatesForeignKey()
    {
        using var scope = Fixture.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = await CreateUserAsync(context);

        context.Add(Membership.Create(userId, Guid.CreateVersion7(), StaffRole.Owner).Value);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        Assert.IsType<PostgresException>(exception.InnerException);
        Assert.Equal("23503", ((PostgresException)exception.InnerException).SqlState);
    }

    [Fact]
    public async Task Delete_CentreWithAMembership_IsRejected()
    {
        using var scope = Fixture.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var centreId = await CreateCentreAsync();
        var userId = await CreateUserAsync(context);
        context.Add(Membership.Create(userId, centreId, StaffRole.Owner).Value);
        await context.SaveChangesAsync();

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var deleteCommand = new NpgsqlCommand("delete from platform.centres where id = @id", connection);
        deleteCommand.Parameters.AddWithValue("id", centreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => deleteCommand.ExecuteNonQueryAsync());
        Assert.Equal("23503", exception.SqlState);
    }

    private async Task<Guid> CreateCentreAsync()
    {
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), command);
```

The excerpt stops at line 90; use the original file for the remaining 19 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
