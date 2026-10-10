using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

// Aliased for the same reason as SubjectUseCaseTests: Task 23.6's test-only RenameSubjectCommand, declared
// directly in this namespace, would otherwise shadow the real (production) one.
using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 25.8: optimistic concurrency proven end to end — the server never substitutes the current version for
/// the client's. No test fetches a fresh version before its second write; that is exactly the anomaly under test.
/// </summary>
public sealed class SubjectStaleVersionTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Rename_WithTheVersionJustRead_Succeeds()
    {
        var actor = await NileOwnerAsync();
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
        var v1 = await ReadVersionAsync(actor, created.Value.SubjectId);

        var result = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", v1));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Rename_AgainWithTheSameStaleVersion_ReturnsConcurrencyStaleAndKeepsTheFirstRename()
    {
        var actor = await NileOwnerAsync();
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
        var v1 = await ReadVersionAsync(actor, created.Value.SubjectId);
        var firstRename = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", v1));
        Assert.True(firstRename.IsSuccess);

        // Still v1 — the version the client originally read, not a freshly re-read one.
        var secondRename = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Pure Mathematics", v1));

        Assert.True(secondRename.IsFailure);
        Assert.Equal("concurrency.stale", secondRename.Error!.Code);
        var current = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(actor, new GetSubjectQuery(created.Value.SubjectId));
        Assert.Equal("Applied Mathematics", current.Value.Name);
    }

    [Fact]
    public async Task Archive_WithAStaleVersion_ReturnsConcurrencyStale()
    {
        var actor = await NileOwnerAsync();
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
        var v1 = await ReadVersionAsync(actor, created.Value.SubjectId);
        await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", v1));

        var archived = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(actor, new ArchiveSubjectCommand(created.Value.SubjectId, v1));

        Assert.True(archived.IsFailure);
        Assert.Equal("concurrency.stale", archived.Error!.Code);
    }

    [Fact]
    public async Task Rename_TwoParallelCarryingTheSameVersion_ExactlyOneSucceeds()
    {
        var actor = await NileOwnerAsync();
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
        var v1 = await ReadVersionAsync(actor, created.Value.SubjectId);
        var gate = new TaskCompletionSource();

        async Task<Result<Unit>> AttemptAsync(string name)
        {
            await gate.Task;
            return await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
                actor, new RealRenameSubjectCommand(created.Value.SubjectId, name, v1));
        }

        var first = Task.Run(() => AttemptAsync("Applied Mathematics"));
        var second = Task.Run(() => AttemptAsync("Pure Mathematics"));
        gate.SetResult();
        var results = await Task.WhenAll(first, second);

        Assert.Equal(1, results.Count(result => result.IsSuccess));
        var loser = Assert.Single(results, result => result.IsFailure);
        Assert.Equal("concurrency.stale", loser.Error!.Code);
    }

    private async Task<StaffActor> NileOwnerAsync() => new(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);

    private async Task<uint> ReadVersionAsync(StaffActor actor, Guid subjectId)
    {
        var dto = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(actor, new GetSubjectQuery(subjectId));
        return dto.Value.Version;
    }
}
