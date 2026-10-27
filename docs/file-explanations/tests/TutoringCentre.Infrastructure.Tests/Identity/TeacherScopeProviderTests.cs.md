# `tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 44.4: the ownership guard resolved through the real provider, the real teacher read service and real PostgreSQL, inside a dispatched request so row-level security is in force. Fails closed: every case that is not explicitly All or Own must come back None.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 134 lines and 5,750 bytes. Source hash: `91a258948886275f70ea2e5b41e8dc1418cd61fe89fd222817709bb2af3e4349`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using TutoringCentre.Application.Academics.Teachers;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;`
- Line 4: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`
- Line 5: `using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;`
- Line 6: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 7: `using TutoringCentre.Application.Common.Cqrs;`
- Line 8: `using TutoringCentre.Application.Common.Security;`
- Line 9: `using TutoringCentre.Domain.Common;`
- Line 10: `using TutoringCentre.Domain.Identity;`
- Line 11: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L20): `public sealed class TeacherScopeProviderTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L23): `public async Task SystemActor_ResolvesAll()`
- [Line 31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L31): `public async Task Owner_ResolvesAll()`
- [Line 41](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L41): `public async Task Secretary_ResolvesAll()`
- [Line 51](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L51): `public async Task TeacherLinkedToActiveRecord_ResolvesOwnWithThatId()`
- [Line 63](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L63): `public async Task TeacherWithNoLinkedRecord_ResolvesNone()`
- [Line 73](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L73): `public async Task TeacherLinkedToArchivedRecord_ResolvesNone()`
- [Line 88](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L88): `public async Task AnonymousActor_ResolvesNone()`
- [Line 95](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L95): `private async Task<TeacherScope> ResolveAsync(Actor actor)`
- [Line 102](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L102): `private async Task<Guid> CreateLinkedTeacherAsync(CreatedStaff owner, Guid userId)`
- [Line 113](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L113): `private async Task<uint> ReadTeacherVersionAsync(CreatedStaff owner, Guid teacherId)`
- [Line 122](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L122): `internal sealed record ResolveTeacherScopeQuery : IQuery<TeacherScope>;`
- [Line 124](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L124): `internal sealed class ResolveTeacherScopeQueryHandler(ITeacherScopeProvider provider) : IQueryHandler<ResolveTeacherScopeQuery, TeacherScope>`
- [Line 126](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L126): `public async Task<Result<TeacherScope>> HandleAsync(ResolveTeacherScopeQuery query, CancellationToken cancellationToken) =>`
- [Line 130](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L130): `internal static class TeacherScopeProviderTestRegistration`
- [Line 132](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L132): `public static IServiceCollection AddTeacherScopeProbe(this IServiceCollection services) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 44.4: the ownership guard resolved through the real provider, the real teacher read service and real
/// PostgreSQL, inside a dispatched request so row-level security is in force. Fails closed: every case that is
/// not explicitly All or Own must come back None.
/// </summary>
public sealed class TeacherScopeProviderTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task SystemActor_ResolvesAll()
    {
        var scope = await ResolveAsync(new SystemActor(NileCentreId));

        Assert.Equal(TeacherScope.All, scope);
    }

    [Fact]
    public async Task Owner_ResolvesAll()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-scope1@nile.test", StaffRole.Owner);

        var scope = await ResolveAsync(owner.Actor);

        Assert.Equal(TeacherScope.All, scope);
    }

    [Fact]
    public async Task Secretary_ResolvesAll()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-scope2@nile.test", StaffRole.Secretary);

        var scope = await ResolveAsync(secretary.Actor);

        Assert.Equal(TeacherScope.All, scope);
    }

    [Fact]
    public async Task TeacherLinkedToActiveRecord_ResolvesOwnWithThatId()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-scope3@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-scope3@nile.test", StaffRole.Teacher);
        var teacherId = await CreateLinkedTeacherAsync(owner, teacherMember.UserId);

        var scope = await ResolveAsync(teacherMember.Actor);

        Assert.Equal(TeacherScope.Own(teacherId), scope);
    }

    [Fact]
    public async Task TeacherWithNoLinkedRecord_ResolvesNone()
    {
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-scope4@nile.test", StaffRole.Teacher);

        var scope = await ResolveAsync(teacherMember.Actor);

        Assert.Equal(TeacherScope.None, scope);
    }

    [Fact]
    public async Task TeacherLinkedToArchivedRecord_ResolvesNone()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-scope5@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-scope5@nile.test", StaffRole.Teacher);
        var teacherId = await CreateLinkedTeacherAsync(owner, teacherMember.UserId);
        var version = await ReadTeacherVersionAsync(owner, teacherId);
        var archived = await Fixture.SendAsAsync<ArchiveTeacherCommand, Unit>(owner.Actor, new ArchiveTeacherCommand(teacherId, version));
        Assert.True(archived.IsSuccess, archived.Error?.Message);

        var scope = await ResolveAsync(teacherMember.Actor);

        Assert.Equal(TeacherScope.None, scope);
    }

    [Fact]
    public async Task AnonymousActor_ResolvesNone()
    {
        var scope = await ResolveAsync(new AnonymousActor());
```

The excerpt stops at line 90; use the original file for the remaining 44 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
