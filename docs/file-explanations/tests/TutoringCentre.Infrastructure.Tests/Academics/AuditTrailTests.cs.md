# `tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 31.7: the audit trail proven through real commands against real PostgreSQL. Every assertion reads audit.audit_entries through the superuser connection (`PostgresFixture.ScalarAsync{T}(string)`), never through the application's own filtered view of it.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 258 lines and 14,073 bytes. Source hash: `98cb48d4771e59f87173d914f988796d2deae6d7051a3e2d7403b6ba4ea78217`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.Configuration;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using TutoringCentre.Application.Academics.Subjects;`
- Line 4: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 5: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 6: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 7: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 8: `using TutoringCentre.Application.Common.Cqrs;`
- Line 9: `using TutoringCentre.Application.Common.Security;`
- Line 10: `using TutoringCentre.Application.Staff;`
- Line 11: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 12: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 13: `using TutoringCentre.Domain.Centres;`
- Line 14: `using TutoringCentre.Domain.Common;`
- Line 15: `using TutoringCentre.Domain.Identity;`
- Line 16: `using TutoringCentre.Infrastructure.Identity;`
- Line 17: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 18: `using TutoringCentre.Infrastructure.Tests.Identity;`
- Line 22: `using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 31](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L31): `public sealed class AuditTrailTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 34](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L34): `public async Task CreateSubject_AsStaffOwner_WritesOneCreatedAuditRow()`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L60): `public async Task RenameSubject_ChangesName_WritesOneUpdatedAuditRowWithNoStatusKey()`
- [Line 84](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L84): `public async Task RenameSubject_ToItsOwnCurrentName_WritesNoAdditionalAuditRow()`
- [Line 99](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L99): `public async Task ArchiveSubject_WritesUpdatedAuditRowWithStatusActiveBeforeAndArchivedAfter()`
- [Line 120](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L120): `public async Task ChangeStaffRole_WritesUpdatedAuditRowWithRoleBeforeAndAfter()`
- [Line 141](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L141): `public async Task CreateStaff_WritesCreatedMembershipAuditRowWithNoPasswordOrEmail()`
- [Line 160](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L160): `public async Task CommandThatRenamesThenFails_WritesNoAuditRowAndNoDataChange()`
- [Line 178](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L178): `public async Task CreateSubject_LosesUniqueRace_WritesNoAuditRowForTheLoser()`
- [Line 203](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L203): `public async Task CreateSubject_AsSystemActorWithCentre_WritesAuditRowWithSystemActorTypeAndNullUser()`
- [Line 219](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L219): `public async Task DevelopmentSeederAndCentrelessCentreCreation_WriteNoAuditRows()`
- [Line 245](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L245): `public async Task AuditEntry_CorrelationId_MatchesTheOneSetOnTheScope()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Identity;

// Task 23.6's test-only RenameSubjectCommand, declared directly in this namespace (SubjectTestRequests.cs),
// would otherwise shadow the real (production) one for the identical simple name.
using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 31.7: the audit trail proven through real commands against real PostgreSQL. Every assertion reads
/// audit.audit_entries through the superuser connection (<see cref="PostgresFixture.ScalarAsync{T}(string)"/>),
/// never through the application's own filtered view of it.
/// </summary>
public sealed class AuditTrailTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task CreateSubject_AsStaffOwner_WritesOneCreatedAuditRow()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-1@nile.test", StaffRole.Owner);

        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        Assert.True(created.IsSuccess);

        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type = 'subject'"));
        Assert.Equal(
            "created",
            await Fixture.ScalarAsync<string>($"select action from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            "Mathematics",
            await Fixture.ScalarAsync<string>($"select changes->'name'->>'after' from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            "staff",
            await Fixture.ScalarAsync<string>($"select actor_type from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            owner.UserId,
            await Fixture.ScalarAsync<Guid>($"select actor_user_id from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
        Assert.Equal(
            NileCentreId,
            await Fixture.ScalarAsync<Guid>($"select centre_id from audit.audit_entries where entity_id = '{created.Value.SubjectId}'"));
    }

    [Fact]
    public async Task RenameSubject_ChangesName_WritesOneUpdatedAuditRowWithNoStatusKey()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-2@nile.test", StaffRole.Owner);
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(owner.Actor, new GetSubjectQuery(created.Value.SubjectId));

        var renamed = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            owner.Actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", dto.Value.Version));
        Assert.True(renamed.IsSuccess);

        var changes = await Fixture.ScalarAsync<string>(
            $"select changes::text from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and action = 'updated'");
        Assert.Equal(
            "Mathematics",
            await Fixture.ScalarAsync<string>(
                $"select changes->'name'->>'before' from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and action = 'updated'"));
        Assert.Equal(
            "Applied Mathematics",
            await Fixture.ScalarAsync<string>(
                $"select changes->'name'->>'after' from audit.audit_entries where entity_id = '{created.Value.SubjectId}' and action = 'updated'"));
        Assert.DoesNotContain("status", changes, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RenameSubject_ToItsOwnCurrentName_WritesNoAdditionalAuditRow()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-3@nile.test", StaffRole.Owner);
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(owner.Actor, new CreateSubjectCommand("Mathematics"));
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(owner.Actor, new GetSubjectQuery(created.Value.SubjectId));
        var countAfterCreate = await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries");

```

The excerpt stops at line 90; use the original file for the remaining 168 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
