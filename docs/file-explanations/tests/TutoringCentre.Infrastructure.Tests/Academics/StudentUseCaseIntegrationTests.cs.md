# `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 40.13: student search, cross-tenant isolation and optimistic-concurrency proofs through the real dispatcher.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 211 lines and 10,506 bytes. Source hash: `2980d6b5985f44b123674a36a3b3020a4d02f862b87d455a5d641b4a95f9f350`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using TutoringCentre.Application.Academics.Students;`
- Line 3: `using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;`
- Line 4: `using TutoringCentre.Application.Academics.Students.Commands.LinkParent;`
- Line 5: `using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;`
- Line 6: `using TutoringCentre.Application.Academics.Students.Queries.GetStudent;`
- Line 7: `using TutoringCentre.Application.Academics.Students.Queries.ListStudents;`
- Line 8: `using TutoringCentre.Application.Common.Cqrs;`
- Line 9: `using TutoringCentre.Application.Common.Security;`
- Line 10: `using TutoringCentre.Domain.Academics;`
- Line 11: `using TutoringCentre.Domain.Identity;`
- Line 12: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 13: `using TutoringCentre.Infrastructure.Tests.Tenancy;`
- Line 14: `using RealRegisterStudentCommand = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentCommand;`
- Line 15: `using RealRegisterStudentParent = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentParent;`
- Line 16: `using RealRegisterStudentResult = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentResult;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L21): `public sealed class StudentUseCaseIntegrationTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L24): `public async Task ListStudents_HamzaInsensitiveSearch_FindsStudentRegisteredWithHamza()`
- [Line 36](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L36): `public async Task ListStudents_PhoneFragment_FindsStudentByOwnPhoneSuffix()`
- [Line 48](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L48): `public async Task ListStudents_PhoneFragment_FindsStudentByParentPhoneSuffix()`
- [Line 60](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L60): `public async Task ListStudents_ArchivedExcludedByDefault_IncludedWhenRequested()`
- [Line 75](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L75): `public async Task ListStudents_KeysetPaging_SecondPageContinuesAfterTheFirst()`
- [Line 93](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L93): `public async Task MaadiActor_CannotSeeOrRegisterAgainstNilesParent()`
- [Line 108](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L108): `public async Task MaadiActor_ListStudents_DoesNotSeeNilesStudent()`
- [Line 121](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L121): `public async Task MaadiActor_GetStudent_SeesNotFoundForNilesStudent()`
- [Line 134](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L134): `public async Task UpdateStudent_StaleVersion_ReturnsConcurrencyStale()`
- [Line 152](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L152): `public async Task LinkParent_StaleVersion_ReturnsConcurrencyStale()`
- [Line 171](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L171): `public async Task RegisterStudent_StudentValidationFailsAfterNewParentWasStaged_NothingPersists()`
- [Line 188](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L188): `private async Task<Guid> RegisterAsync(`
- [Line 200](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L200): `private async Task<Guid> RegisterParentOnlyAsync(Actor actor)`
- [Line 207](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L207): `private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Owner);`
- [Line 209](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs#L209): `private async Task<uint> ReadVersionAsync(Guid studentId) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Globalization;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;
using RealRegisterStudentCommand = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentCommand;
using RealRegisterStudentParent = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentParent;
using RealRegisterStudentResult = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentResult;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>Task 40.13: student search, cross-tenant isolation and optimistic-concurrency proofs through the real dispatcher.</summary>
public sealed class StudentUseCaseIntegrationTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task ListStudents_HamzaInsensitiveSearch_FindsStudentRegisteredWithHamza()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await RegisterAsync(actor, "أحمد حسن");

        var result = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("احمد", false, null, 25));

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value.Items, item => item.FullName == "أحمد حسن");
    }

    [Fact]
    public async Task ListStudents_PhoneFragment_FindsStudentByOwnPhoneSuffix()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await RegisterAsync(actor, "Youssef Omar", studentPhone: "01112345678");

        var result = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("5678", false, null, 25));

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value.Items, item => item.FullName == "Youssef Omar");
    }

    [Fact]
    public async Task ListStudents_PhoneFragment_FindsStudentByParentPhoneSuffix()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await RegisterAsync(actor, "Sara Ali", parentPhone: "01098765432");

        var result = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("5432", false, null, 25));

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value.Items, item => item.FullName == "Sara Ali");
    }

    [Fact]
    public async Task ListStudents_ArchivedExcludedByDefault_IncludedWhenRequested()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var studentId = await RegisterAsync(actor, "Mona Adel");
        var version = await ReadVersionAsync(studentId);
        await Fixture.SendAsAsync<ArchiveStudentCommand, Unit>(actor, new ArchiveStudentCommand(studentId, version));

        var activeOnly = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("Mona", false, null, 25));
        var includingArchived = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery("Mona", true, null, 25));

        Assert.DoesNotContain(activeOnly.Value.Items, item => item.Id == studentId);
        Assert.Contains(includingArchived.Value.Items, item => item.Id == studentId);
    }

    [Fact]
    public async Task ListStudents_KeysetPaging_SecondPageContinuesAfterTheFirst()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        await RegisterAsync(actor, "Aya Tarek");
        await RegisterAsync(actor, "Bassem Nabil");
        await RegisterAsync(actor, "Cherif Adel");

        var firstPage = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(actor, new ListStudentsQuery(null, false, null, 2));
        Assert.Equal(2, firstPage.Value.Items.Count);
        Assert.NotNull(firstPage.Value.NextCursor);

        var secondPage = await Fixture.QueryAsAsync<ListStudentsQuery, StudentPageDto>(
            actor, new ListStudentsQuery(null, false, firstPage.Value.NextCursor, 2));

        Assert.DoesNotContain(secondPage.Value.Items, item => firstPage.Value.Items.Select(first => first.Id).Contains(item.Id));
    }
```

The excerpt stops at line 90; use the original file for the remaining 121 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
