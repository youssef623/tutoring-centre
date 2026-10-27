# `tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 43.14: the account link is the security-relevant part of Task 43.8, so most cases here are refusals rather than the happy path alone — an unknown user, a user of another centre, a user with another role and an inactive member all answer teacher.user_not_eligible identically through the real IStaffDirectory adapter and real PostgreSQL; a user already linked answers teacher.account_taken, including under a real race that only the unique index can arbitrate.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 188 lines and 10,078 bytes. Source hash: `b3c0fe1c7c3f9c59da95a0512e9ee46eec17ba6fbd47d744445743a22a257a1b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers;`
- Line 2: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;`
- Line 4: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 5: `using TutoringCentre.Application.Common.Cqrs;`
- Line 6: `using TutoringCentre.Application.Common.Security;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Domain.Identity;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 10: `using TutoringCentre.Infrastructure.Tests.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L21): `public sealed class TeacherAccountLinkTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L24): `public async Task SetAccount_EligibleTeacherRoleUser_Links()`
- [Line 39](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L39): `public async Task SetAccount_SecretaryUserId_ReturnsUserNotEligible()`
- [Line 52](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L52): `public async Task SetAccount_MaadiOnlyUser_ReturnsUserNotEligible()`
- [Line 65](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L65): `public async Task SetAccount_UnknownUserId_ReturnsUserNotEligible()`
- [Line 77](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L77): `public async Task SetAccount_UserAlreadyLinkedToAnotherTeacher_ReturnsAccountTaken()`
- [Line 97](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L97): `public async Task SetAccount_NullUserId_UnlinksAPreviouslyLinkedTeacher()`
- [Line 116](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L116): `public async Task SetAccount_TwoParallelLinksOfOneUserToTwoTeachers_OneSucceedsOneAccountTaken()`
- [Line 148](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L148): `public async Task SetAccount_Links_WritesOneUpdatedAuditRowWithUserIdBeforeNullAndAfterTheId()`
- [Line 171](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L171): `private static void AssertUserNotEligible<T>(Result<T> result)`
- [Line 179](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L179): `private async Task<CreatedTeacherRecord> CreateTeacherAsync(CreatedStaff owner, string fullName)`
- [Line 187](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L187): `private sealed record CreatedTeacherRecord(Guid TeacherId, uint Version);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Identity;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 43.14: the account link is the security-relevant part of Task 43.8, so most cases here are refusals
/// rather than the happy path alone — an unknown user, a user of another centre, a user with another role
/// and an inactive member all answer teacher.user_not_eligible identically through the real IStaffDirectory
/// adapter and real PostgreSQL; a user already linked answers teacher.account_taken, including under a real
/// race that only the unique index can arbitrate.
/// </summary>
public sealed class TeacherAccountLinkTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task SetAccount_EligibleTeacherRoleUser_Links()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link1@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-link1@nile.test", StaffRole.Teacher);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, teacherMember.UserId, teacherRecord.Version));

        Assert.True(result.IsSuccess);
        var dto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(owner.Actor, new GetTeacherQuery(teacherRecord.TeacherId));
        Assert.Equal(teacherMember.UserId, dto.Value.UserId);
    }

    [Fact]
    public async Task SetAccount_SecretaryUserId_ReturnsUserNotEligible()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link2@nile.test", StaffRole.Owner);
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-link2@nile.test", StaffRole.Secretary);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, secretary.UserId, teacherRecord.Version));

        AssertUserNotEligible(result);
    }

    [Fact]
    public async Task SetAccount_MaadiOnlyUser_ReturnsUserNotEligible()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link3@nile.test", StaffRole.Owner);
        var maadiOnlyTeacher = await CreateStaffAsync(MaadiCentreId, "maadi-only-link3@maadi.test", StaffRole.Teacher);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, maadiOnlyTeacher.UserId, teacherRecord.Version));

        AssertUserNotEligible(result);
    }

    [Fact]
    public async Task SetAccount_UnknownUserId_ReturnsUserNotEligible()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link4@nile.test", StaffRole.Owner);
        var teacherRecord = await CreateTeacherAsync(owner, "Amina Hassan");

        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(teacherRecord.TeacherId, Guid.CreateVersion7(), teacherRecord.Version));

        AssertUserNotEligible(result);
    }

    [Fact]
    public async Task SetAccount_UserAlreadyLinkedToAnotherTeacher_ReturnsAccountTaken()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-link5@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-link5@nile.test", StaffRole.Teacher);
        var firstRecord = await CreateTeacherAsync(owner, "Amina Hassan");
        var linked = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(firstRecord.TeacherId, teacherMember.UserId, firstRecord.Version));
        Assert.True(linked.IsSuccess);

        var secondRecord = await CreateTeacherAsync(owner, "Mona Said");
        var result = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(secondRecord.TeacherId, teacherMember.UserId, secondRecord.Version));

        Assert.True(result.IsFailure);
```

The excerpt stops at line 90; use the original file for the remaining 98 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
