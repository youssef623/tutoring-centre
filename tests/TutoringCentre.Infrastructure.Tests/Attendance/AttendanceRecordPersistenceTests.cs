using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class AttendanceRecordPersistenceTests(PostgresFixture fixture) : AttendanceTestBase(fixture)
{
    [Fact]
    public async Task NileActorCreatesRecord_PersistsTenantAndTimestamp()
    {
        var actor = await ActorAsync(NileCentreId);
        var id = await CreateAsync(actor, await SeedReferencesAsync(NileCentreId));
        var record = await ReadAsync(actor, id);
        Assert.NotNull(record);
        Assert.Equal(NileCentreId, record.CentreId);
        Assert.Equal(0, record.CorrectionCount);
        Assert.Equal(AttendanceStatus.Present, record.Status);
        Assert.Equal(AttendanceSource.Manual, record.Source);
        Assert.Equal(actor.UserId, record.MarkedByUserId);
        Assert.Equal(1, await Fixture.ScalarAsync<long>($"""
            select count(*) from attendance.attendance_records where id = '{id}'
            and centre_id = '{NileCentreId}' and created_at is not null and updated_at is null and correction_count = 0
            """));
    }

    [Fact]
    public async Task MaadiActorCannotReadNileRecord()
    {
        var id = await CreateAsync(await ActorAsync(NileCentreId), await SeedReferencesAsync(NileCentreId));
        Assert.Null(await ReadAsync(await ActorAsync(MaadiCentreId), id));
    }

    [Fact]
    public async Task DuplicateFirstMark_TranslatesToConcurrentMark_AndKeepsOneRow()
    {
        var actor = await ActorAsync(NileCentreId);
        var references = await SeedReferencesAsync(NileCentreId);
        await CreateAsync(actor, references);
        var result = await Fixture.SendAsAsync<AddAttendanceCommand, Guid>(actor, new(references));
        Assert.True(result.IsFailure);
        Assert.Equal("attendance.concurrent_mark", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from attendance.attendance_records"));
    }

    [Fact]
    public async Task TwoContextsUpdatingOneRecord_SecondGetsStaleConflict()
    {
        var actor = await ActorAsync(NileCentreId);
        var id = await CreateAsync(actor, await SeedReferencesAsync(NileCentreId));
        await using var firstScope = Fixture.Services.CreateAsyncScope();
        await using var secondScope = Fixture.Services.CreateAsyncScope();
        var (firstUnit, firstDb) = await BeginAsync(firstScope, actor);
        var (secondUnit, secondDb) = await BeginAsync(secondScope, actor);
        var first = await firstDb.Set<AttendanceRecord>().SingleAsync(record => record.Id == id);
        var second = await secondDb.Set<AttendanceRecord>().SingleAsync(record => record.Id == id);
        Assert.True(first.Correct(AttendanceStatus.Absent, "first correction", actor.UserId,
            AttendanceTestTiming.Timing, AttendanceTestTiming.Now).IsSuccess);
        await firstDb.SaveChangesAsync();
        await firstUnit.CommitAsync(CancellationToken.None);
        Assert.True(second.Correct(AttendanceStatus.Late, "second correction", actor.UserId,
            AttendanceTestTiming.Timing, AttendanceTestTiming.Now).IsSuccess);
        var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secondDb.SaveChangesAsync());
        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);
        Assert.NotNull(result);
        Assert.Equal("concurrency.stale", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        await secondUnit.RollbackAsync(CancellationToken.None);
        var stored = await ReadAsync(actor, id);
        Assert.NotNull(stored);
        Assert.Equal(AttendanceStatus.Absent, stored.Status);
        Assert.Equal("first correction", stored.LastCorrectionReason);
        Assert.Equal(1, await AuditCountAsync(id));
    }

    [Fact]
    public async Task FirstMarkWritesNoAuditRow()
    {
        var id = await CreateAsync(await ActorAsync(NileCentreId), await SeedReferencesAsync(NileCentreId));
        Assert.Equal(0, await AuditCountAsync(id));
    }

    [Fact]
    public async Task CorrectionWritesOneUpdatedAuditRow_WithBeforeAfterAndReason()
    {
        var actor = await ActorAsync(NileCentreId);
        var id = await CreateAsync(actor, await SeedReferencesAsync(NileCentreId));
        var before = await ReadAsync(actor, id);
        var result = await Fixture.SendAsAsync<CorrectAttendanceCommand, Unit>(actor,
            new(id, AttendanceStatus.Absent, "  wrong   student "));
        Assert.True(result.IsSuccess);
        var stored = await ReadAsync(actor, id);
        Assert.NotNull(stored);
        Assert.Equal("wrong student", stored.LastCorrectionReason);
        Assert.Equal(1, stored.CorrectionCount);
        Assert.Equal(before!.MarkedAt, stored.MarkedAt);
        Assert.Equal(before.MarkedByUserId, stored.MarkedByUserId);
        Assert.Equal(before.Source, stored.Source);
        Assert.Equal(actor.UserId, stored.LastCorrectedByUserId);
        Assert.Equal(1, await AuditCountAsync(id));
        Assert.Equal("updated", await Fixture.ScalarAsync<string>($"select action from audit.audit_entries where entity_id = '{id}'"));
        using var document = JsonDocument.Parse(await Fixture.ScalarAsync<string>($"select changes::text from audit.audit_entries where entity_id = '{id}'"));
        var changes = document.RootElement;
        Assert.Equal(["correctionCount", "correctionReason", "status"], changes.EnumerateObject().Select(property => property.Name).Order().ToArray());
        Assert.Equal("present", changes.GetProperty("status").GetProperty("before").GetString());
        Assert.Equal("absent", changes.GetProperty("status").GetProperty("after").GetString());
        Assert.Equal(JsonValueKind.Null, changes.GetProperty("correctionReason").GetProperty("before").ValueKind);
        Assert.Equal("wrong student", changes.GetProperty("correctionReason").GetProperty("after").GetString());
    }

    [Fact]
    public async Task NileActorSavingMaadiRecord_IsRejectedByWriteGuard()
    {
        var actor = await ActorAsync(NileCentreId);
        var references = await SeedReferencesAsync(MaadiCentreId);
        await Assert.ThrowsAsync<TenantViolationException>(() =>
            Fixture.SendAsAsync<AddAttendanceCommand, Guid>(actor, new(references)));
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from attendance.attendance_records"));
    }

    private async Task<StaffActor> ActorAsync(Guid centre) => new(await Fixture.SeedUserAsync(), centre, StaffRole.Secretary);

    private async Task<Guid> CreateAsync(StaffActor actor, AttendanceReferences references)
    {
        var result = await Fixture.SendAsAsync<AddAttendanceCommand, Guid>(actor, new(references));
        Assert.True(result.IsSuccess, result.Error?.Code);
        return result.Value;
    }

    private async Task<AttendanceRecord?> ReadAsync(StaffActor actor, Guid id)
    {
        var result = await Fixture.QueryAsAsync<GetAttendanceQuery, AttendanceRecord?>(actor, new(id));
        Assert.True(result.IsSuccess);
        return result.Value;
    }

    private Task<long> AuditCountAsync(Guid id) => Fixture.ScalarAsync<long>(
        $"select count(*) from audit.audit_entries where entity_type = 'attendance_record' and entity_id = '{id}'");

    private static async Task<(IUnitOfWork Unit, AppDbContext Db)> BeginAsync(AsyncServiceScope scope, Actor actor)
    {
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(readOnly: false, CancellationToken.None);
        return (unit, scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }
}
