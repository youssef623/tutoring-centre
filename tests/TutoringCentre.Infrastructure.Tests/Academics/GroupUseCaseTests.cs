using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 44.14: the six group commands through the real dispatcher, real adapters and real PostgreSQL, plus the
/// two places the database, not the handler, must arbitrate: a name that differs only by hamza form, and ten
/// parallel creates racing past the handler's own uniqueness check.
/// </summary>
public sealed class GroupUseCaseTests(PostgresFixture fixture) : GroupTestBase(fixture)
{
    private const int Attempts = 10;

    [Fact]
    public async Task Create_ThenGet_ReturnsTheGroupWithSubjectAndTeacherNames()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-guc1@nile.test", StaffRole.Owner);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var teacherId = await CreateTeacherAsync(owner.Actor, "Amina Hassan");

        var groupId = await CreateGroupAsync(owner.Actor, "  Grade   10 A ", subjectId, teacherId, 25);

        var group = await GetGroupAsync(owner.Actor, groupId);
        Assert.Equal("Grade 10 A", group.Name);
        Assert.Equal(subjectId, group.SubjectId);
        Assert.Equal("Mathematics", group.SubjectName);
        Assert.Equal(teacherId, group.TeacherId);
        Assert.Equal("Amina Hassan", group.TeacherName);
        Assert.Equal(25, group.Capacity);
        Assert.Equal(0, group.EnrolledCount);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public async Task Rename_PersistsTheNewName()
    {
        var (owner, groupId, _, _) = await CreateOneGroupAsync("owner-guc2@nile.test");
        var version = (await GetGroupAsync(owner.Actor, groupId)).Version;

        var result = await Fixture.SendAsAsync<RenameGroupCommand, Unit>(owner.Actor, new RenameGroupCommand(groupId, "Grade 10 B", version));

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal("Grade 10 B", (await GetGroupAsync(owner.Actor, groupId)).Name);
    }

    [Fact]
    public async Task Rename_WithAStaleVersion_ReturnsConcurrencyStale()
    {
        var (owner, groupId, _, _) = await CreateOneGroupAsync("owner-guc3@nile.test");
        var staleVersion = (await GetGroupAsync(owner.Actor, groupId)).Version;
        var first = await Fixture.SendAsAsync<RenameGroupCommand, Unit>(owner.Actor, new RenameGroupCommand(groupId, "Grade 10 B", staleVersion));
        Assert.True(first.IsSuccess, first.Error?.Message);

        var result = await Fixture.SendAsAsync<RenameGroupCommand, Unit>(owner.Actor, new RenameGroupCommand(groupId, "Grade 10 C", staleVersion));

        Assert.True(result.IsFailure);
        Assert.Equal("concurrency.stale", result.Error!.Code);
        Assert.Equal("Grade 10 B", (await GetGroupAsync(owner.Actor, groupId)).Name);
    }

    [Fact]
    public async Task ChangeCapacity_PersistsInRangeAndRefusesOutOfRange()
    {
        var (owner, groupId, _, _) = await CreateOneGroupAsync("owner-guc4@nile.test");
        var version = (await GetGroupAsync(owner.Actor, groupId)).Version;

        var changed = await Fixture.SendAsAsync<ChangeGroupCapacityCommand, Unit>(owner.Actor, new ChangeGroupCapacityCommand(groupId, 200, version));
        var refused = await Fixture.SendAsAsync<ChangeGroupCapacityCommand, Unit>(
            owner.Actor, new ChangeGroupCapacityCommand(groupId, 201, (await GetGroupAsync(owner.Actor, groupId)).Version));

        Assert.True(changed.IsSuccess, changed.Error?.Message);
        Assert.Equal("group.capacity_invalid", refused.Error!.Code);
        Assert.Equal(200, (await GetGroupAsync(owner.Actor, groupId)).Capacity);
    }

    [Fact]
    public async Task AssignTeacher_MovesTheGroupAndAuditsTeacherIdBeforeAndAfter()
    {
        var (owner, groupId, _, firstTeacherId) = await CreateOneGroupAsync("owner-guc5@nile.test");
        var secondTeacherId = await CreateTeacherAsync(owner.Actor, "Youssef Kamal");
        var version = (await GetGroupAsync(owner.Actor, groupId)).Version;

        var result = await Fixture.SendAsAsync<AssignGroupTeacherCommand, Unit>(
            owner.Actor, new AssignGroupTeacherCommand(groupId, secondTeacherId, version));

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(secondTeacherId, (await GetGroupAsync(owner.Actor, groupId)).TeacherId);
        var auditFilter = $"from audit.audit_entries where entity_id = '{groupId}' and entity_type = 'group' and action = 'updated'";
        Assert.Equal(firstTeacherId.ToString(), await Fixture.ScalarAsync<string>($"select changes->'teacherId'->>'before' {auditFilter}"));
        Assert.Equal(secondTeacherId.ToString(), await Fixture.ScalarAsync<string>($"select changes->'teacherId'->>'after' {auditFilter}"));
    }

    [Fact]
    public async Task ArchiveThenRestore_KeepsTheRowAndTogglesStatus()
    {
        var (owner, groupId, _, _) = await CreateOneGroupAsync("owner-guc6@nile.test");

        var archived = await Fixture.SendAsAsync<ArchiveGroupCommand, Unit>(
            owner.Actor, new ArchiveGroupCommand(groupId, (await GetGroupAsync(owner.Actor, groupId)).Version));
        Assert.True(archived.IsSuccess, archived.Error?.Message);
        Assert.Equal(GroupStatus.Archived, (await GetGroupAsync(owner.Actor, groupId)).Status);
        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from academics.groups where id = '{groupId}'"));

        var restored = await Fixture.SendAsAsync<RestoreGroupCommand, Unit>(
            owner.Actor, new RestoreGroupCommand(groupId, (await GetGroupAsync(owner.Actor, groupId)).Version));
        Assert.True(restored.IsSuccess, restored.Error?.Message);
        Assert.Equal(GroupStatus.Active, (await GetGroupAsync(owner.Actor, groupId)).Status);
    }

    [Fact]
    public async Task Restore_UnderAnArchivedSubject_ReturnsSubjectArchived()
    {
        var (owner, groupId, subjectId, _) = await CreateOneGroupAsync("owner-guc7@nile.test");
        var archivedGroup = await Fixture.SendAsAsync<ArchiveGroupCommand, Unit>(
            owner.Actor, new ArchiveGroupCommand(groupId, (await GetGroupAsync(owner.Actor, groupId)).Version));
        Assert.True(archivedGroup.IsSuccess, archivedGroup.Error?.Message);
        var archivedSubject = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(
            owner.Actor, new ArchiveSubjectCommand(subjectId, await SubjectVersionAsync(owner.Actor, subjectId)));
        Assert.True(archivedSubject.IsSuccess, archivedSubject.Error?.Message);

        var result = await Fixture.SendAsAsync<RestoreGroupCommand, Unit>(
            owner.Actor, new RestoreGroupCommand(groupId, (await GetGroupAsync(owner.Actor, groupId)).Version));

        Assert.Equal("group.subject_archived", result.Error!.Code);
        Assert.Equal(GroupStatus.Archived, (await GetGroupAsync(owner.Actor, groupId)).Status);
    }

    [Fact]
    public async Task Create_NameDifferingOnlyByHamzaForm_ReturnsNameTaken()
    {
        var (owner, _, subjectId, teacherId) = await CreateOneGroupAsync("owner-guc8@nile.test", "مجموعة أحمد");

        var result = await Fixture.SendAsAsync<CreateGroupCommand, CreateGroupResult>(
            owner.Actor, new CreateGroupCommand("مجموعة احمد", subjectId, teacherId, 20));

        Assert.True(result.IsFailure);
        Assert.Equal("group.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from academics.groups"));
    }

    [Fact]
    public async Task Create_TenParallelCreatesWithOneName_LeavesExactlyOneRow()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-guc9@nile.test", StaffRole.Owner);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var teacherId = await CreateTeacherAsync(owner.Actor, "Amina Hassan");
        var gate = new TaskCompletionSource();

        async Task<Result<CreateGroupResult>> AttemptAsync()
        {
            await gate.Task;
            return await Fixture.SendAsAsync<CreateGroupCommand, CreateGroupResult>(
                owner.Actor, new CreateGroupCommand("Grade 10 A", subjectId, teacherId, 20));
        }

        var tasks = Enumerable.Range(0, Attempts).Select(_ => Task.Run(AttemptAsync)).ToArray();
        gate.SetResult();
        var results = await Task.WhenAll(tasks);

        Assert.Equal(1, results.Count(result => result.IsSuccess));
        Assert.All(results.Where(result => result.IsFailure), result => Assert.Equal("group.name_taken", result.Error!.Code));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from academics.groups"));
    }

    [Fact]
    public async Task TeacherRead_ActiveGroupCount_CountsOnlyActiveGroups()
    {
        var (owner, firstGroupId, subjectId, teacherId) = await CreateOneGroupAsync("owner-guc10@nile.test");
        await CreateGroupAsync(owner.Actor, "Grade 10 B", subjectId, teacherId);
        var archived = await Fixture.SendAsAsync<ArchiveGroupCommand, Unit>(
            owner.Actor, new ArchiveGroupCommand(firstGroupId, (await GetGroupAsync(owner.Actor, firstGroupId)).Version));
        Assert.True(archived.IsSuccess, archived.Error?.Message);

        var teacher = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(owner.Actor, new GetTeacherQuery(teacherId));

        Assert.Equal(1, teacher.Value.ActiveGroupCount);
    }

    [Fact]
    public async Task ArchiveSubjectAndTeacher_WithAnActiveGroup_AreRefused()
    {
        var (owner, _, subjectId, teacherId) = await CreateOneGroupAsync("owner-guc11@nile.test");

        var subjectResult = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(
            owner.Actor, new ArchiveSubjectCommand(subjectId, await SubjectVersionAsync(owner.Actor, subjectId)));
        var teacherResult = await Fixture.SendAsAsync<ArchiveTeacherCommand, Unit>(
            owner.Actor, new ArchiveTeacherCommand(teacherId, await TeacherVersionAsync(owner.Actor, teacherId)));

        Assert.Equal("subject.has_active_groups", subjectResult.Error!.Code);
        Assert.Equal(ErrorKind.Rule, subjectResult.Error.Kind);
        Assert.Equal("teacher.has_active_groups", teacherResult.Error!.Code);
        Assert.Equal(ErrorKind.Rule, teacherResult.Error.Kind);
    }

    private async Task<(CreatedStaff Owner, Guid GroupId, Guid SubjectId, Guid TeacherId)> CreateOneGroupAsync(
        string ownerEmail, string groupName = "Grade 10 A")
    {
        var owner = await CreateStaffAsync(NileCentreId, ownerEmail, StaffRole.Owner);
        var subjectId = await CreateSubjectAsync(owner.Actor, "Mathematics");
        var teacherId = await CreateTeacherAsync(owner.Actor, "Amina Hassan");
        var groupId = await CreateGroupAsync(owner.Actor, groupName, subjectId, teacherId);
        return (owner, groupId, subjectId, teacherId);
    }
}
