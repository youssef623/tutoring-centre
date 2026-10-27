# `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs`

## Purpose

Declares `StudentReadServicePhoneSmokeTest`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 66 lines and 3,109 bytes. Source hash: `e8bd9ec971184e46c266bf6e3d9fd6137e4ad52e97709f11f1348c31f9ed3ef4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 3: `using Npgsql;`
- Line 4: `using TutoringCentre.Application.Academics.Students;`
- Line 5: `using TutoringCentre.Application.Common.Security;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Persistence;`
- Line 8: `using TutoringCentre.Infrastructure.ReadServices;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 10: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs#L14): `public sealed class StudentReadServicePhoneSmokeTest(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs#L17): `public async Task PhoneFragmentSearch_FindsStudentByParentPhoneSuffix()`
- [Line 46](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs#L46): `public async Task NameSearch_LogsPredicateParameterAndLimit()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.ReadServices;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

public sealed class StudentReadServicePhoneSmokeTest(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task PhoneFragmentSearch_FindsStudentByParentPhoneSuffix()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Omar Hassan");
        await using (var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString))
        {
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand($"update academics.students set phone = '+201012345678' where id = '{studentId}'", connection);
            await command.ExecuteNonQueryAsync();
        }

        var actor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner);
        var actorContext = new CurrentActorContext();
        actorContext.Set(actor);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(Fixture.AppConnectionString).UseSnakeCaseNamingConvention();
        await using var db = new AppDbContext(optionsBuilder.Options, actorContext);
        var unitOfWork = new UnitOfWork(db, NullLogger<UnitOfWork>.Instance);
        await unitOfWork.BeginAsync(readOnly: true, CancellationToken.None);

        var readService = new StudentReadService(db);
        var result = await readService.ListAsync(new StudentSearchCriteria("5678", false, null, 25), CancellationToken.None);

        await unitOfWork.CommitAsync(CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(studentId, result.Items[0].Id);
    }

    [Fact]
    public async Task NameSearch_LogsPredicateParameterAndLimit()
    {
        await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Hassan");

        var actor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner);
        var actorContext = new CurrentActorContext();
        actorContext.Set(actor);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(Fixture.AppConnectionString).UseSnakeCaseNamingConvention();
        await using var db = new AppDbContext(optionsBuilder.Options, actorContext);
        var unitOfWork = new UnitOfWork(db, NullLogger<UnitOfWork>.Instance);
        await unitOfWork.BeginAsync(readOnly: true, CancellationToken.None);

        var readService = new StudentReadService(db);
        var result = await readService.ListAsync(new StudentSearchCriteria("hassan", false, null, 25), CancellationToken.None);

        await unitOfWork.CommitAsync(CancellationToken.None);

        Assert.Single(result.Items);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
