using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.IssueStudentCard;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Repositories;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class CardReissueRaceTests(PostgresFixture fixture) : AttendanceCommandTestBase(fixture)
{
    [Fact]
    public async Task ParallelFirstIssues_IndexPreventsTwoActiveCards()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "card-first-race");
        var gate = new ReadGate();
        await using var provider = Fixture.CreateServiceProvider(services =>
        {
            services.AddSingleton<TimeProvider>(Time);
            services.AddScoped<IStudentCardRepository>(scope => new GatedCards(new StudentCardRepository(scope.GetRequiredService<AppDbContext>()), gate));
        });
        var results = await Task.WhenAll(
            provider.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId)),
            provider.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId)));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from attendance.student_cards where status = 'active'"));
        Assert.Single(results, result => result.IsSuccess);
        Assert.Equal("card.concurrent_issue", Assert.Single(results, result => result.IsFailure).Error!.Code);
    }

    [Fact]
    public async Task ParallelReissues_TenRaces_OneSuccessOneConflictOneActive()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "card-race");
        Assert.True((await Clocked.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId))).IsSuccess);
        var gate = new ReadGate();
        await using var provider = Fixture.CreateServiceProvider(services =>
        {
            services.AddSingleton<TimeProvider>(Time);
            services.AddScoped<IStudentCardRepository>(scope => new GatedCards(new StudentCardRepository(scope.GetRequiredService<AppDbContext>()), gate));
        });
        for (var attempt = 0; attempt < 10; attempt++)
        {
            gate.Reset();
            var results = await Task.WhenAll(
                provider.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId)),
                provider.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId)));
            Assert.Single(results, result => result.IsSuccess);
            Assert.Equal("card.concurrent_issue", Assert.Single(results, result => result.IsFailure).Error!.Code);
            Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from attendance.student_cards where student_id = '{scenario.StudentId}' and status = 'active'"));
        }
        Assert.Equal(11, await Fixture.ScalarAsync<long>("select count(*) from attendance.student_cards"));
    }

    private sealed class ReadGate
    {
        private int readers;
        private TaskCompletionSource ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public void Reset() { readers = 0; ready = new(TaskCreationOptions.RunContinuationsAsynchronously); }
        public async Task Arrive(CancellationToken ct)
        {
            if (Interlocked.Increment(ref readers) == 2) ready.SetResult();
            await ready.Task.WaitAsync(TimeSpan.FromSeconds(20), ct);
        }
    }

    private sealed class GatedCards(IStudentCardRepository inner, ReadGate gate) : IStudentCardRepository
    {
        public async Task<StudentCard?> GetActiveAsync(Guid studentId, CancellationToken ct)
        {
            var card = await inner.GetActiveAsync(studentId, ct);
            await gate.Arrive(ct);
            return card;
        }
        public Task<IReadOnlyList<StudentCard>> GetActiveAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken ct) => inner.GetActiveAsync(studentIds, ct);
        public Task<StudentCard?> GetByIdAsync(Guid cardId, uint expectedVersion, CancellationToken ct) => inner.GetByIdAsync(cardId, expectedVersion, ct);
        public Task<StudentCard?> FindByTokenHashAsync(byte[] tokenHash, CancellationToken ct) => inner.FindByTokenHashAsync(tokenHash, ct);
        public void Add(StudentCard card) => inner.Add(card);
    }
}
